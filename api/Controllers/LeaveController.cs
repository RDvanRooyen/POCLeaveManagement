using data.Database;
using data.Model;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Model;

namespace api.Controllers
{
    [ApiController]
    [Route("api/Leaves")]
    public class LeaveController : ControllerBase
    {
        private readonly LeaveDBContext _leaveDbContext;
        private IConfiguration _configuration;

        public LeaveController(LeaveDBContext leaveDBContext, IConfiguration configuration)
        {
            _leaveDbContext = leaveDBContext;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<ApiResult<int>> Create([FromBody] CreateLeaveRequest request,
            CancellationToken cancellationToken)
        {
            var employee = await _leaveDbContext.Employees.SingleOrDefaultAsync(x => x.Id == request.EmployeeId,
                cancellationToken: cancellationToken);
            if (employee == null)
            {
                return ApiResult<int>.Failure("Employee not found");
            }

            if (request.LeaveStart.HasValue == false || request.LeaveEnd.HasValue == false)
            {
                return ApiResult<int>.Failure("Leave start and end are required");
            }

            DateTime startDate = Convert.ToDateTime(request.LeaveStart);
            DateTime endDate = Convert.ToDateTime(request.LeaveEnd);
            int daysLeave = (endDate - startDate).Days;

            if (daysLeave < 0)
            {
                return ApiResult<int>.Failure("Leave end date can't be before leave start date");
            }

            switch (request.TypeOfLeave)
            {
                case "Annual Leave":
                    if (employee.AnnualLeaveRemaining < daysLeave + 1)
                    {
                        return ApiResult<int>.Failure($"{employee.Name} doesn't have enough annual leave");
                    }
                    break;

                case "Sick Leave":
                    if (employee.SickLeaveRemaining < daysLeave + 1)
                    {
                        return ApiResult<int>.Failure($"{employee.Name} doesn't have enough sick leave");
                    }
                    break;

                case "Family Responsibility Leave":
                    if (employee.FamRespLeaveRemaining < daysLeave + 1)
                    {
                        return ApiResult<int>.Failure($"{employee.Name} doesn't have enough family responsibility leave");
                    }
                    break;
            }

            Leave leave = new();
            leave.Employee = employee;

            leave.LeaveStart = DateOnly.FromDateTime(request.LeaveStart.Value);
            leave.LeaveEnd = DateOnly.FromDateTime(request.LeaveEnd.Value);
            leave.TypeOfLeave = request.TypeOfLeave;
            leave.Reason = request.Reason;
            _leaveDbContext.Leaves.Add(leave);
            int rowsAffected = await _leaveDbContext.SaveChangesAsync(cancellationToken);
            if (rowsAffected > 0)
            {
                switch (leave.TypeOfLeave)
                {
                    case "Annual Leave":
                        employee.AnnualLeaveRemaining = employee.AnnualLeaveRemaining -
                                                        (1 + leave.LeaveEnd.DayNumber - leave.LeaveStart.DayNumber);
                        break;

                    case "Sick Leave":
                        employee.SickLeaveRemaining = employee.SickLeaveRemaining -
                                                      (1 + leave.LeaveEnd.DayNumber - leave.LeaveStart.DayNumber);
                        break;

                    case "Family Responsibility Leave":
                        employee.FamRespLeaveRemaining = employee.FamRespLeaveRemaining -
                                                         (1 + leave.LeaveEnd.DayNumber - leave.LeaveStart.DayNumber);
                        break;
                }
                _leaveDbContext.Employees.Update(employee);
                rowsAffected = await _leaveDbContext.SaveChangesAsync(cancellationToken);
                if (rowsAffected > 0)
                {
                    return await AddCalenderEvent(leave.Id, employee.Id);
                }
            }

            return ApiResult<int>.Failure($"Failed to add {employee.Name}'s leave");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<List<LeaveModel>> List(CancellationToken cancellationToken)
        {
            var leave = await _leaveDbContext.Leaves.Include(x => x.Employee)
                .ToListAsync(cancellationToken: cancellationToken);
            return leave.Select(x => new LeaveModel()
            {
                Id = x.Id,
                Employee = new EmployeeModel()
                {
                    Id = x.Id,
                    Name = x.Employee.Name,
                    LastName = x.Employee.LastName,
                    SickLeaveRemaining = x.Employee.SickLeaveRemaining,
                    FamRespLeaveRemaining = x.Employee.FamRespLeaveRemaining,
                    AnnualLeaveRemaining = x.Employee.AnnualLeaveRemaining
                },
                LeaveStart = x.LeaveStart.ToDateTime(TimeOnly.MinValue),
                LeaveEnd = x.LeaveEnd.ToDateTime(TimeOnly.MinValue),
                TypeOfLeave = x.TypeOfLeave,
                Reason = x.Reason,
            }).ToList();
        }

        public async Task<ApiResult<int>> AddCalenderEvent(int leaveId, int employeeId)
        {
            string clientId = this._configuration.GetSection("AppSettings")["ClientId"];
            string clientSecret = this._configuration.GetSection("AppSettings")["ClientSecret"];
            string site = this._configuration.GetSection("AppSettings")["Site"];
            var employeeDetail = await _leaveDbContext.Employees.SingleOrDefaultAsync(x => x.Id == employeeId);
            var leaveDetail = await _leaveDbContext.Leaves.SingleOrDefaultAsync(x => x.Id == leaveId);
            if (employeeDetail == null || leaveDetail == null)
            {
                return ApiResult<int>.Failure($"{employeeDetail.Name}'s leave added successfully, leave not added to calendar");
            }
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            new ClientSecrets
                            {
                                ClientId = clientId,
                                ClientSecret = clientSecret,
                            },
                            new[] { CalendarService.Scope.Calendar }, "user", CancellationToken.None).Result;

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "POC Leave Management",
            });

            Event myEvent = new Event
            {
                Summary = String.Format("{0}, {1}.", String.Join(" ", employeeDetail.Name, employeeDetail.LastName),
                          leaveDetail.TypeOfLeave),
                Description = String.Format("{0} is on {1} from : {2} to {3}. \nNote: \n{4}.", String.Join(" ", employeeDetail.Name, employeeDetail.LastName),
                          leaveDetail.TypeOfLeave, leaveDetail.LeaveStart, leaveDetail.LeaveEnd, leaveDetail.Reason),
                Start = new EventDateTime()
                {
                    DateTime = leaveDetail.LeaveStart.ToDateTime(TimeOnly.Parse("12:00:00 AM")),
                    TimeZone = "Africa/Johannesburg"
                },
                End = new EventDateTime()
                {
                    DateTime = leaveDetail.LeaveEnd.ToDateTime(TimeOnly.Parse("11:59:59 PM")),
                    TimeZone = "Africa/Johannesburg"
                }
            };
            service.Events.Insert(myEvent, "primary").Execute();
            return ApiResult<int>.Success(leaveDetail.Id, $"{employeeDetail.Name}'s leave added successfully with calendar entry");
        }
    }
}