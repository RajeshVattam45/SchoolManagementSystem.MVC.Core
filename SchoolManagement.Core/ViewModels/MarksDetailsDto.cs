using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class MarksDetailsDto
    {
        public int MarkId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;

        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string ExamName { get; set; } = string.Empty;
        public string ExamTypeName { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }

        public string Result { get; set; } = string.Empty;
    }
}
