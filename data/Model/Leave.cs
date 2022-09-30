using System.ComponentModel.DataAnnotations;

namespace data.Model
{
    public class Leave
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Employee can not be null")]
        public Employee Employee { get; set; }
        [Required(ErrorMessage = "Leave start date can not be null")]
        public DateOnly LeaveStart { get; set; }
        [Required(ErrorMessage = "Leave end date can not be null")]
        public DateOnly LeaveEnd { get; set; }
        [Required(ErrorMessage = "Type of eave can not be null")]
        public string TypeOfLeave { get; set; }
        public string Reason { get; set; }
    }
}
