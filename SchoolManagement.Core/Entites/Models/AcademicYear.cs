using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Entites.Models
{
    public class AcademicYear
    {
        public int Id { get; set; }

        [Required ( ErrorMessage = "Academic year name is required." )]
        public string YearName { get; set; }

        [Required ( ErrorMessage = "Start date is required." )]
        public DateTime StartDate { get; set; }

        [Required ( ErrorMessage = "End date is required." )]
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; }

        //public ICollection<Student> Students { get; set; } = new List<Student> ();
    }
}
