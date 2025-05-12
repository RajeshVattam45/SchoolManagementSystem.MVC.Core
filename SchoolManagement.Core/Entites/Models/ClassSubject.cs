using SchoolManagement.Core.Entites.Models;
using System.Text.Json.Serialization;

namespace SchoolManagement.Core.Entites.Models
{
    public class ClassSubject
    {
        public int Id { get; set; }

        // Foreign Key for Class
        public int? ClassId { get; set; }
        [JsonIgnore]
        public Class? Class { get; set; }

        // Foreign Key for Subject
        public int? SubjectId { get; set; }
        [JsonIgnore]
        public Subject? Subject { get; set; }
    }
}
