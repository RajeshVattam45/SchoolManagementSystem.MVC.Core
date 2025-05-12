using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolManagement.Core.Entites.Models
{
    public class ExamType
    {
        [Key]
        public int Id { get; set; }
        public int? ExamTypeId { get; set; }
        public string ExamTypeName { get; set; }
        public int MaxMarks { get; set; }
        [JsonIgnore]
        public ICollection<Exam>? Exams { get; set; }
    }
}
