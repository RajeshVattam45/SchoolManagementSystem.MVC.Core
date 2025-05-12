using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entites.Models
{
    public class StudentFees
    {
        [Key]
        public int FeeId { get; set; }

        [ForeignKey( "Student" )]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        [Column ( TypeName = "decimal(18,2)" )]
        public decimal TotalPaidAmount { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        public DateTime DueDate { get; set; }
    }
}
