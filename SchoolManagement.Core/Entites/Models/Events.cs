using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Entites.Models
{
    public class Events
    {
        [Key]
        public int Id { get; set; }

        public string EventName { get; set; }

        public DateTime EventDate { get; set; }

        public string EventDescription { get; set; }

        public string OrganizedBy { get; set; }
    }
}
