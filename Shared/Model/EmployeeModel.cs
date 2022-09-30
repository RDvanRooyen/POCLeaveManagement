namespace Shared.Model
    {
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int AnnualLeaveRemaining { get; set; }
        public int FamRespLeaveRemaining { get; set; }
        public int SickLeaveRemaining { get; set; }
    }
}