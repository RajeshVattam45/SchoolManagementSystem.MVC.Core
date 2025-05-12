using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class ExamDisplayDto
    {
        public string ExamTypeName { get; set; }
        public string ExamName { get; set; }
        public DateTime ExamDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string RoomNumber { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
    }

}
