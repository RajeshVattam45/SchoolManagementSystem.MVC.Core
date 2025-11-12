using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ViewModels
{
    public class ExamScheduleBatchRequest
    {
        public int ClassId { get; set; }
        public List<ExamScheduleEntry> Schedules { get; set; }

        public class ExamScheduleEntry
        {
            public int ExamId { get; set; }
            public int SubjecId { get; set; }
            public DateTime ExamDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string RoomNumber { get; set; }
        }
    }
}
