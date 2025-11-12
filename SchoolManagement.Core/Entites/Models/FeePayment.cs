using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Entites.Models
{
    public class FeePayment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("Student")]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        [Required]
        [StringLength ( 50 )]
        public string FeeType { get; set; }

        [Column ( TypeName = "decimal(18,2)" )]
        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; }

    }
}
