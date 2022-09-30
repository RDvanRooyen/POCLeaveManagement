namespace Shared.Model
{
    public class CreateEmployeeRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int AnnualLeaveRemaining { get; set; }
        public int FamRespLeaveRemaining { get; set; }
        public int SickLeaveRemaining { get; set; }
    }
}
