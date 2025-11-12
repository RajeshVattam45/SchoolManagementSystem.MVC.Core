using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class MarksEditDto
    {
        public int MarkId { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }
    }

}
