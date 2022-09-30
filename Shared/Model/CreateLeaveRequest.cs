using data.Model;

namespace Shared.Model
{
    public class CreateLeaveRequest
    {
        public int EmployeeId { get; set; } = -1;
        public DateTime? LeaveStart { get; set; } = null;
        public DateTime? LeaveEnd { get; set; } = null;
        public string TypeOfLeave { get; set; }
        public string Reason { get; set; }
    }
}
