namespace SchoolManagement.Core.ViewModels
{
    public class StudentAttendanceGroupedViewModel
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public List<DailyAttendance> AttendanceRecords { get; set; }
    }
}
