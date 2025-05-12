namespace SchoolManagement.Core.ViewModels
{
    public class DailyAttendance
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public DateTime Date { get; set; }
        public string Class { get; set; }
        public string Status { get; set; }
    }
}
