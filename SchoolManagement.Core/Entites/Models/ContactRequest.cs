using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.Entites.Models
{
    public class ContactRequest
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "Name required")]
        public string Name { get; set; }

        [Required (ErrorMessage = "Email Required")]
        [EmailAddress ( ErrorMessage = "Invalid email address format." )]
        public string Email { get; set; }

        [Required (ErrorMessage = "Message required")]
        public string Message { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }

}
