using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class MarksDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public int ExamTypeId { get; set; }
        public string ExamTypeName { get; set; }
        public DateTime? ExamDate { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }
    }
}
