using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SchoolManagement.Core.Entites.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required (ErrorMessage = "Student Id is required")]
        public int? StudentId { get; set; }

        [Required(ErrorMessage ="First Name Is Required")]
        [StringLength ( 50, ErrorMessage = "First name cannot exceed 50 characters." )]
        public string FirstName { get; set; }

        [Required( ErrorMessage = "Last Name Is Required" )]
        [StringLength ( 50, ErrorMessage = "Last name cannot exceed 50 characters." )]
        public string LastName { get; set; }

        [Required ( ErrorMessage = "Phone number is required." )]
        [RegularExpression ( @"^\d{10}$", ErrorMessage = "Invalid phone number format. It should be exactly 10 digits." )]
        public string PhoneNumber { get; set; }

        //[Required]
        public string? Role { get; set; }

        [Required (ErrorMessage ="Address required")]
        [StringLength ( 100, ErrorMessage = "Permanent address cannot exceed 100 characters." )]
        public string PermanentAddress { get; set; }

        [StringLength ( 100, ErrorMessage = "Current address cannot exceed 100 characters." )]
        public string CurrentAddress { get; set; }

        [Required (ErrorMessage ="Pin code is required")]
        [RegularExpression ( @"^\d{6}$", ErrorMessage = "Pincode must be a 6-digit number." )]
        public string Pincode { get; set; }

        [Required (ErrorMessage ="Age is required")]
        [Range ( 1, 20, ErrorMessage = "Age must be between 1 and 15." )]
        public int Age { get; set; }

        [Required (ErrorMessage ="Please enter Date Of Birth")]
        [DataType ( DataType.Date )]
        public DateTime DateOfBirth { get; set; }

        [StringLength ( 100, ErrorMessage = "Previous school name cannot exceed 100 characters." )]
        public string? PreviousSchool { get; set; }

        public string? PreviousSchoolClass { get; set; }

        [Range ( 0, 100, ErrorMessage = "Percentage must be between 0 and 100." )]
        public float? PreviousSchoolPercentage { get; set; }

        //[Required (ErrorMessage = "Email required")]
        [EmailAddress ( ErrorMessage = "Invalid email address format." )]
        public string? Email { get; set; }

        //[Required(ErrorMessage = "Password required")]
        [DataType ( DataType.Password )]
        public string? PasswordHash { get; set; }

        [Required (ErrorMessage = "Please Enter Class")]
        [ForeignKey ( "Class" )]
        public int? ClassId { get; set; }
        [JsonIgnore]
        public Class? Class { get; set; }


        [Required ( ErrorMessage = "Gender should be Male, Female, or Other." )]
        [StringLength(10, ErrorMessage = "Gender should be Male, Female, or Other.")]
        public string? Gender { get; set; }

        [StringLength(20, ErrorMessage = "Academic Year cannot exceed 20 characters.")]
        public string? AcademicYear { get; set; }

        [Required (ErrorMessage = "Must Enter Date Of Join")]
        [DataType(DataType.Date)]
        public DateTime DateOfJoining { get; set; }

        public byte[]? ProfileImage { get; set; }

        [StringLength ( 50, ErrorMessage = "Religion cannot exceed 50 characters." )]
        public string? Religion { get; set; }

        [StringLength ( 50, ErrorMessage = "Caste cannot exceed 50 characters." )]
        public string? Caste { get; set; }

        [StringLength ( 10, ErrorMessage = "Blood group cannot exceed 10 characters." )]
        public string? BloodGroup { get; set; }

        public ICollection<Guardian> Guardians { get; set; } = new List<Guardian> ();

        public ICollection<StudentFees>? StudentFees { get; set; }
        public ICollection<FeePayment>? FeePayments { get; set; }
        public ICollection<Marks>? Marks { get; set; }
        public ICollection<StudentAttendance>? StudentAttendance { get; set; }
        public ICollection<LibraryTransactions>? LibraryTransactions { get; set; }
        public ICollection<StudentClassHistory>? StudentClassHistories { get; set; }
    }
}
