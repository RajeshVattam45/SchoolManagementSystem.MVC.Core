using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolManagement.Core.Entites.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range ( 1, 10, ErrorMessage = "Class must be between 1 and 10." )]
        public int ClassId { get; set; }
        public string? ClassName { get; set; }
        public decimal? TotalFee { get; set; }

        // Relationship: One Class -> Many Students
        public ICollection<Student>? Students { get; set; }
        public ICollection<ClassSubjectTeacher>? ClassSubjectTeachers { get; set; }
        [JsonIgnore]
        public ICollection<ExamSchedule>? ExamSchedules { get; set; }
        public ICollection<StudentFees>? StudentFees { get; set; }
        public ICollection<Marks>? Marks { get; set; }
        public ICollection<StudentAttendance>? StudentAttendances { get; set; }
        public ICollection<FeePayment>? FeePayments { get; set; }
        public ICollection<Timetable>? Timetables { get; set; }
        public ICollection<StudentClassHistory>? StudentClassHistories { get; set; }
        public ICollection<ClassSubject>? ClassSubjects { get; set; }
    }
}
