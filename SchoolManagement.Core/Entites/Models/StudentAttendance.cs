using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entites.Models
{
    public class StudentAttendance
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public DateOnly Date {  get; set; }

        public string Status { get; set; }

        [ForeignKey("Class")]
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        //internal object Select ( Func<object, DailyAttendance> value )
        //{
        //    throw new NotImplementedException ();
        //}
    }
}
