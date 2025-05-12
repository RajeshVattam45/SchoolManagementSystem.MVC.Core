namespace SchoolManagement.Core.ViewModels
{
    // DTO class for handling attendance updates
    public class StudentAttendanceDto
    {
        public int StudentId { get; set; }
        public string Status { get; set; }
        public DateOnly Date { get; set; }
    }
}
