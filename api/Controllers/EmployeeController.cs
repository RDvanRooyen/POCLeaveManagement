using data.Database;
using data.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Model;

namespace api.Controllers
{
    [ApiController]
    [Route("api/Employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly LeaveDBContext _leaveDbContext;

        public EmployeeController(LeaveDBContext leaveDbContext)
        {
            _leaveDbContext = leaveDbContext;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ApiResult<int>> Create([FromBody] CreateEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            Employee employee = new();
            employee.Name = request.Name;
            employee.LastName = request.LastName;
            employee.AnnualLeaveRemaining = request.AnnualLeaveRemaining;
            employee.SickLeaveRemaining = request.SickLeaveRemaining;
            employee.FamRespLeaveRemaining = request.FamRespLeaveRemaining;
            _leaveDbContext.Employees.Add(employee);
            int rowsAffected = await _leaveDbContext.SaveChangesAsync(cancellationToken);
            if (rowsAffected > 0)
            {
                return ApiResult<int>.Success(employee.Id, $"New Employee, {request.Name}, created successfully");
            }

            return ApiResult<int>.Failure($"Failed to create {request.Name}");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<List<EmployeeModel>> AutocompleteEmployees([FromQuery] AutocompleteEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            var couldParse = long.TryParse(request.Input, out long parsedValue);
            if (couldParse)
            {
                var candidate = _leaveDbContext.Employees.FirstOrDefault(x => x.Id == parsedValue);
                if (candidate != null)
                {
                    return new List<EmployeeModel>()
                    {
                        new EmployeeModel()
                        {
                            Id = candidate.Id,
                            Name = candidate.Name,
                            LastName = candidate.LastName,
                            SickLeaveRemaining = candidate.SickLeaveRemaining,
                            FamRespLeaveRemaining = candidate.FamRespLeaveRemaining,
                            AnnualLeaveRemaining = candidate.AnnualLeaveRemaining
                        }
                    };
                }
            }

            var queryable = _leaveDbContext.Employees.Where(x =>
                x.Name.Contains(request.Input) || x.LastName.Contains(request.Input));
            var seperatedValues = request.Input.Split(" ");
            if (seperatedValues.Length > 1)
            {
                queryable = _leaveDbContext.Employees;
                foreach (var seperatedValue in seperatedValues)
                {
                    queryable = queryable.Where(x =>
                        x.Name.Contains(seperatedValue) || x.LastName.Contains(seperatedValue));
                }
            }

            var results = await queryable
                .ToListAsync(cancellationToken: cancellationToken);
            return results.Select(x => new EmployeeModel()
            {
                Id = x.Id,
                Name = x.Name,
                LastName = x.LastName,
                SickLeaveRemaining = x.SickLeaveRemaining,
                FamRespLeaveRemaining = x.FamRespLeaveRemaining,
                AnnualLeaveRemaining = x.AnnualLeaveRemaining
            }).ToList();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<List<EmployeeModel>> List(CancellationToken cancellationToken)
        {
            var employee = await _leaveDbContext.Employees.ToListAsync(cancellationToken: cancellationToken);
            return employee.Select(x => new EmployeeModel()
            {
                Id = x.Id,
                Name = x.Name,
                LastName = x.LastName,
                SickLeaveRemaining = x.SickLeaveRemaining,
                FamRespLeaveRemaining = x.FamRespLeaveRemaining,
                AnnualLeaveRemaining = x.AnnualLeaveRemaining
            }).ToList();
        }
    }
}