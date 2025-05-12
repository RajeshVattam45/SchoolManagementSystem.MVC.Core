using SchoolManagement.Core.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class StudentGuardianViewModel
    {
        public Student Student { get; set; } = new Student ();
        public List<Guardian> Guardians { get; set; } = new List<Guardian> { new Guardian () };
    }
}
