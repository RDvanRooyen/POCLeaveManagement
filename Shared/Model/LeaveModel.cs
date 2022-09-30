using data.Model;

namespace Shared.Model
{
    public class LeaveModel
    {
        public int Id { get; set; }
        public EmployeeModel Employee { get; set; }
        public DateTime LeaveStart { get; set; }
        public DateTime LeaveEnd { get; set; }
        public string TypeOfLeave { get; set; }
        public string Reason { get; set; }
    }
}