using DinkToPdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using System.Reflection.Metadata;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolManagement.Core.ViewModels;


namespace SchoolManagement.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService ( IStudentRepository studentRepository )
        {
            _studentRepository = studentRepository;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task RegisterStudentAsync ( Student student )
        {
            try
            {
                string password = student.PasswordHash;

                if (string.IsNullOrWhiteSpace ( password ))
                {
                    throw new ArgumentException ( "Password is required." );
                }

                if (password.Length < 7)
                {
                    throw new ArgumentException ( "Password must be at least 7 characters long." );
                }

                if (!password.Any ( char.IsUpper ))
                {
                    throw new ArgumentException ( "Password must contain at least one uppercase letter." );
                }

                if (!password.Any ( char.IsLower ))
                {
                    throw new ArgumentException ( "Password must contain at least one lowercase letter." );
                }

                if (!password.Any ( ch => !char.IsLetterOrDigit ( ch ) ))
                {
                    throw new ArgumentException ( "Password must contain at least one special character." );
                }

                if (password.Contains ( " " ))
                {
                    throw new ArgumentException ( "Password must not contain spaces." );
                }

                var passwordHasher = new PasswordHasher<Student> ();
                student.PasswordHash = passwordHasher.HashPassword ( student, password );

                // All validations passed, proceed with registration
                await _studentRepository.RegisterStudentAsync ( student );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Student>> GetAllStudentsAsync ( )
        {
            return await _studentRepository.GetAllStudentsAsync ();
        }

        public async Task<Student> GetStudentByIdAsync ( int id )
        {
            return await _studentRepository.GetStudentByIdAsync ( id );
        }

        public async Task<List<Guardian>> GetGuardiansByStudentIdAsync ( int studentId )
        {
            return await _studentRepository.GetGuardiansByStudentIdAsync ( studentId );
        }

        public async Task<bool> UpdateStudentAsync ( int id, StudentGuardianViewModel viewModel )
        {
            return await _studentRepository.UpdateStudentAsync ( id, viewModel );
        }

        public async Task<bool> DeleteStudentAsync ( int id )
        {
            return await _studentRepository.DeleteStudentAsync ( id );
        }
        public async Task<Student> GetStudentByEmailAsync ( string email )
        {
            return await _studentRepository.GetStudentByEmailAsync ( email );
        }

        public async Task PromoteStudentAsync ( int studentId, int newClassId )
        {
            await _studentRepository.PromoteStudentAsync ( studentId, newClassId );
        }

        public async Task<bool> ChangePasswordAsync ( int studentId, string oldPassword, string newPassword )
        {
            var student = await _studentRepository.GetStudentByIdAsync ( studentId );
            if (student == null)
                throw new ArgumentException ( "Student not found." );

            var passwordHasher = new PasswordHasher<Student> ();

            var result = passwordHasher.VerifyHashedPassword ( student, student.PasswordHash, oldPassword );
            if (result != PasswordVerificationResult.Success)
                throw new ArgumentException ( "Old password is incorrect." );

            // Password validation
            if (string.IsNullOrWhiteSpace ( newPassword ) || newPassword.Length < 7 ||
                !newPassword.Any ( char.IsUpper ) || !newPassword.Any ( char.IsLower ) ||
                !newPassword.Any ( ch => !char.IsLetterOrDigit ( ch ) ) || newPassword.Contains ( " " ))
            {
                throw new ArgumentException ( "New password does not meet the security requirements." );
            }

            string newHash = passwordHasher.HashPassword ( student, newPassword );

            return await _studentRepository.ChangePasswordAsync ( studentId, newHash );
        }

        public async Task<byte[]> GenerateStudentDetailsPdfAsync ( int studentId )
        {
            var student = await _studentRepository.GetStudentByIdAsync ( studentId );

            if (student == null)
                throw new ArgumentException ( "Student not found" );

            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create ( container =>
            {
                container.Page ( page =>
                {
                    page.Margin ( 30 );
                    page.Size ( PageSizes.A4 );
                    page.PageColor ( Colors.White );
                    page.DefaultTextStyle ( x => x.FontSize ( 12 ).FontFamily ( "Arial" ) );

                    page.Header ().Row ( row =>
                    {
                        row.ConstantItem ( 100 ).Height ( 100 ).Image ( student.ProfileImage );
                        row.RelativeItem ().Column ( col =>
                        {
                            col.Item ().Text ( $"{student.FirstName} {student.LastName}" ).Bold ().FontSize ( 18 );
                            col.Item ().Text ( $"Email: {student.Email}" );
                            col.Item ().Text ( $"Role: Student" );
                            col.Item ().Text ( $"DOB: {student.DateOfBirth.ToString ( "dd-MM-yyyy" ) ?? "N/A"}" );
                            col.Item ().Text ( $"Class: {student.Class?.ClassName ?? "N/A"}" );
                        } );

                        row.RelativeItem ().Column ( col =>
                        {
                            col.Item ().Text ( $"Phone: {student.PhoneNumber}" );
                            col.Item ().Text ( $"Gender: {student.Gender}" );
                            col.Item ().Text ( $"Age: {student.Age}" );
                            col.Item ().Text ( $"Academic Year: {student.AcademicYear}" );
                        } );
                    } );

                    page.Content ().PaddingVertical ( 20 ).Column ( col =>
                    {
                        col.Item ().Text ( "Address & Family Info" ).Bold ().FontSize ( 16 ).FontColor ( Colors.Blue.Medium );
                        col.Item ().LineHorizontal ( 1 ).LineColor ( Colors.Grey.Lighten2 );

                        col.Item ().Row ( row =>
                        {
                            row.RelativeItem ().Column ( left =>
                            {
                                left.Item ().Text ( $"Permanent Address: {student.PermanentAddress ?? "-"}" );
                                left.Item ().Text ( $"Current Address: {student.CurrentAddress ?? "-"}" );
                                left.Item ().Text ( $"Pincode: {student.Pincode ?? "-"}" );
                                left.Item ().Text ( $"Religion: {student.Religion ?? "-"}" );
                                left.Item ().Text ( $"Caste: {student.Caste ?? "-"}" );
                                left.Item ().Text ( $"Blood Group: {student.BloodGroup ?? "-"}" );
                            } );

                            //row.RelativeItem ().Column ( right =>
                            //{
                            //    right.Item ().Text ( $"Father's Name: {student.FatherName ?? "-"}" );
                            //    right.Item ().Text ( $"Father's Phone: {student.FatherPhoneNumber ?? "-"}" );
                            //    right.Item ().Text ( $"Father's Occupation: {student.FatherOccupation ?? "-"}" );
                            //    right.Item ().Text ( $"Mother's Name: {student.MotherName ?? "-"}" );
                            //    right.Item ().Text ( $"Mother's Occupation: {student.MotherOccupation ?? "-"}" );
                            //} );
                        } );
                    } );

                    page.Footer ().AlignCenter ().Text ( $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}" );
                } );
            } );

            return document.GeneratePdf ();
        }

        public async Task RegisterStudentWithGuardiansAsync ( StudentGuardianViewModel model )
        {
            var student = model.Student;

            // Validate academic logic
            ValidateStudentAcademicPlacement ( student );

            // Password validation and hashing
            string password = student.PasswordHash ?? throw new ArgumentException ( "Password is required." );

            if (password.Length < 7 || !password.Any ( char.IsUpper ) || !password.Any ( char.IsLower ) ||
                !password.Any ( ch => !char.IsLetterOrDigit ( ch ) ) || password.Contains ( " " ))
            {
                throw new ArgumentException ( "Password does not meet complexity requirements." );
            }

            var passwordHasher = new PasswordHasher<Student> ();
            student.PasswordHash = passwordHasher.HashPassword ( student, password );

            // Add to database
            await _studentRepository.RegisterStudentWithGuardiansAsync ( student, model.Guardians );
        }

        public async Task<Student> GetStudentByStudentIdAsync ( int studentId )
        {
            return await _studentRepository.GetStudentByStudentIdAsync ( studentId );
        }

        public void ValidateStudentAcademicPlacement ( Student student )
        {
            int age = DateTime.Today.Year - student.DateOfBirth.Year;
            if (student.DateOfBirth > DateTime.Today.AddYears ( -age )) age--;

            // Basic age range validation
            if (age < 5 || age > 14)
                throw new ArgumentException ( "Student age must be between 5 and 14 years." );

            // Strict age-to-class mapping
            int expectedClass = age - 4;

            if (student.ClassId != expectedClass)
                throw new ArgumentException ( $"A student aged {age} must be enrolled in Class {expectedClass}." );

            // Previous school required from age 6 (i.e., Class 2) and above
            if (age >= 6)
            {
                ValidatePreviousSchoolDetails ( student );
            }

            // Safety: ensure ClassId is within supported range
            if (student.ClassId > 10)
                throw new ArgumentException ( "This system currently supports up to Class 10." );
        }

        private void ValidatePreviousSchoolDetails ( Student student )
        {
            if (string.IsNullOrWhiteSpace ( student.PreviousSchool ) ||
                string.IsNullOrWhiteSpace ( student.PreviousSchoolClass ) ||
                student.PreviousSchoolPercentage <= 0)
            {
                throw new ArgumentException ( "Previous school details are required for Class 2 and above." );
            }

            if (!int.TryParse ( student.PreviousSchoolClass, out int previousClass ))
            {
                throw new ArgumentException ( "Previous School Class must be a number." );
            }

            if (previousClass != student.ClassId - 1)
            {
                throw new ArgumentException ( $"Previous class must be Class {student.ClassId - 1}, but got Class {previousClass}." );
            }
        }
    }
}
