using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SchoolManagement.Core.Entites.Models
{
    public class Guardian
    {
        public int Id { get; set; }

        [Required]
        [StringLength ( 50 )]
        public string GuardianName { get; set; }

        [Required]
        [StringLength ( 20 )]
        public string Relationship { get; set; }

        [StringLength ( 50 )]
        public string? Occupation { get; set; }

        [Required]
        [RegularExpression ( @"^\d{10}$", ErrorMessage = "Must be a valid 10-digit number." )]
        public string PhoneNumber { get; set; }

        [ForeignKey ( "Student" )]
        public int StudentId { get; set; }

        public Student? Student { get; set; }

    }
}
