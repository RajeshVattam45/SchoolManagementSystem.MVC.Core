using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagement.Core.Entites.Models
{
    public class Subject
    {
        [Key]
        [DatabaseGenerated ( DatabaseGeneratedOption.Identity )]
        public int Id { get; set; }

        public int? SubjectId { get; set; }

        [Required]
        public string SubjectName { get; set; }

        // [JsonIgnore]
        [ForeignKey ( "Employee" )]
        public int? EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public ICollection<ClassSubjectTeacher>? ClassSubjectTeachers { get; set; }
        [JsonIgnore]
        public ICollection<ExamSchedule>? ExamSchedules { get; set; }
        public ICollection<Marks>? Marks { get; set; }
        public ICollection<Timetable>? Timetables { get; set; }
        public ICollection<ClassSubject>? ClassSubjects { get; set; }
        [JsonIgnore]
        public ICollection<ExamSubject>? ExamSubjects { get; set; }
    }
}
