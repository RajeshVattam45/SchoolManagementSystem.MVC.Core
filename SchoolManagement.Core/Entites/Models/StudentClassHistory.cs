using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Entites.Models
{
    public class StudentClassHistory
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
       
        [ForeignKey ( "Student" )]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        [ForeignKey ( "Class" )]
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        [Required]
        [StringLength ( 20, ErrorMessage = "Academic Year cannot exceed 20 characters." )]
        public string AcademicYear { get; set; }

        [Required]
        [DataType ( DataType.Date )]
        public DateTime EnrollmentDate { get; set; }

        [DataType ( DataType.Date )]
        public DateTime? CompletionDate { get; set; }
    }
}
