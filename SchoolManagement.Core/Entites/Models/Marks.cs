using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Entites.Models
{
    public class Marks
    {
        [Key]
        public int MarkId { get; set; }

        [ForeignKey ( "Student" )]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        [ForeignKey ( "Exam" )]
        public int? ExamId { get; set; }
        public Exam? Exam { get; set; }

        [ForeignKey ( "Subject" )]
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }

        [ForeignKey ( "Class" )]
        public int? ClassId { get; set; }
        public Class? Class { get; set; }
    }
}
