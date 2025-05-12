using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class AuthResponse
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public int StudentId { get; set; }
        public string Token { get; set; }
    }
}
