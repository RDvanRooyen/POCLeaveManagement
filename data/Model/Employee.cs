using System.ComponentModel.DataAnnotations;

namespace data.Model
{
    public class Employee
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Employee name can not be null")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Employee last name can not be null")]
        public string LastName { get; set; }
        public int AnnualLeaveRemaining { get; set; }
        public int FamRespLeaveRemaining { get; set; }
        public int SickLeaveRemaining { get; set; }
        public virtual ICollection<Leave> Leaves { get; set; }
    }
}
