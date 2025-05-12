using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.Core.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class AssignSubjectToExamViewModel
    {
        public int SelectedExamId { get; set; }
        public List<int> SelectedSubjectIds { get; set; } = new List<int> ();

        // For listing available subjects in the view.
        public List<Subject> Subjects { get; set; } = new List<Subject> ();

        // For populating the exam dropdown.
        public List<SelectListItem> ExamList { get; set; } = new List<SelectListItem> ();
    }
}
