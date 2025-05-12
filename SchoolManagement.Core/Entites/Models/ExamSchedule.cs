using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagement.Core.Entites.Models
{
    public class ExamSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [ForeignKey ( "Exam" )]
        public int? ExamId { get; set; }
        
        public Exam? Exam { get; set; }

        [ForeignKey ( "Class" )]
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        [ForeignKey ( "Subject" )]
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public DateTime ExamDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string RoomNumber { get; set; }
    }
}
