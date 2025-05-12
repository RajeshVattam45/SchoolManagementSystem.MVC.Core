using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entites.Models
{
    public class ExamSubject
    {
        [Key]
        [DatabaseGenerated ( DatabaseGeneratedOption.Identity )]
        public int Id { get; set; }

        // Foreign Key - Exam
       
        [ForeignKey ( "Exam" )]
        public int? ExamId { get; set; }

        public Exam? Exam { get; set; }

        // Foreign Key - Subject
        [Required]
        [ForeignKey ( "Subject" )]
        public int? SubjectId { get; set; }

        public Subject? Subject { get; set; }
    }
}
