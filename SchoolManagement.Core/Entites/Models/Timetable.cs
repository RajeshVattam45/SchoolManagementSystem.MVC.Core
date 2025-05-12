using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entites.Models
{
    public class Timetable
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        [ForeignKey("Subject")]
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        [ForeignKey("Employee")]
        public int? EmployeeID { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        [StringLength ( 20 )]
        public string DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
