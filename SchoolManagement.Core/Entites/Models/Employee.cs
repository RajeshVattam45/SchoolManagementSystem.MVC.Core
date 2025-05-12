using SchoolManagement.Core.Entites.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagement.Core.Entites.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Column ( "EmployeeID" )]
        public int EmployeeId { get; set; }

        [Required (ErrorMessage = "First name required")]
        public string FirstName { get; set; }

        [Required ( ErrorMessage = "Last name required" )]
        public string LastName { get; set; }

        [Required ( ErrorMessage = "Phone number is required." )]
        [RegularExpression ( @"^\d{10}$", ErrorMessage = "Invalid phone number format. It should be exactly 10 digits." )]
        public string PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string PermanentAddress { get; set; }
        public string CurrentAddress { get; set; }

        [Required]
        [RegularExpression ( @"^\d{6}$", ErrorMessage = "Pincode must be a 6-digit number." )]
        public string Pincode { get; set; }
        public string? EmployeeType { get; set; }
        public decimal EmployeeSalary { get; set; }

        [EmailAddress ( ErrorMessage = "Invalid email address format." )]
        public string? Email { get; set; }

        [DataType ( DataType.Password )]
        public string? PasswordHash { get; set; }

        [Required]
        [StringLength ( 10, ErrorMessage = "Gender should be Male, Female, or Other." )]
        public string Gender { get; set; }

        [StringLength ( 20, ErrorMessage = "Academic Year cannot exceed 20 characters." )]
        public string? AcademicYear { get; set; }

        [Required]
        [DataType ( DataType.Date )]
        public DateTime DateOfJoining { get; set; }

        public byte[]? ProfileImage { get; set; }

        // Relationship: One Employee -> Many Subjects
        [JsonIgnore]
        public ICollection<Subject>? Subjects { get; set; }
        public ICollection<ClassSubjectTeacher>? ClassSubjectTeachers { get; set; }
        public ICollection<EmployeeAttendance>? employeeAttendances { get; set; }
        public ICollection<Timetable>? Timetables { get; set; }
    }
}
