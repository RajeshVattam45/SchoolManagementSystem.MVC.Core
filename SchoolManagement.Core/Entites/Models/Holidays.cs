using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Entites.Models
{
    public class Holidays
    {
        [Key]
        public int HolidayId { get; set; }

        public string HolidayName { get; set; }

        public int HolidayYear { get; set; }

        public DateTime HolidayDate { get; set; }

        public string Description { get; set; }
    }
}
