using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entites.Models
{
    public class LibraryTransactions
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey( "LibraryBooks" )]
        public int? BookId { get; set; }
        public LibraryBooks? LibraryBooks { get; set; }

        [ForeignKey( "Student" )]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }
    }
}
