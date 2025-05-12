using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class RegisterMarksDto
    {
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
    }
}
