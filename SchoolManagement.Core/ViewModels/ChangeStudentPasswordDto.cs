using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class ChangeStudentPasswordDto
    {
        public int StudentId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
