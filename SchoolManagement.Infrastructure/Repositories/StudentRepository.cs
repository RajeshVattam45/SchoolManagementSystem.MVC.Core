using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ViewModels;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SchoolDbContext _context;

        public StudentRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task RegisterStudentAsync ( Student student )
        {
            _context.Students.Add ( student );
            await _context.SaveChangesAsync ();
        }

        public async Task<List<Student>> GetAllStudentsAsync ( )
        {
            return await _context.Students
                .Include ( s => s.Class )
                .ToListAsync ();
        }

        public async Task<Student> GetStudentByIdAsync ( int id )
        {
            return await _context.Students.FindAsync ( id );
        }

        public async Task<List<Guardian>> GetGuardiansByStudentIdAsync ( int studentId )
        {
            return await _context.Guardians
                                 .Where ( g => g.StudentId == studentId )
                                 .ToListAsync ();
        }

        public async Task<bool> UpdateStudentAsync ( int id, StudentGuardianViewModel viewModel )
        {
            var existingStudent = await _context.Students
                .Include ( s => s.Guardians )
                .FirstOrDefaultAsync ( s => s.Id == id );

            if (existingStudent == null)
                return false;

            // Update student fields
            _context.Entry ( existingStudent ).CurrentValues.SetValues ( viewModel.Student );

            // Update guardians
            if (viewModel.Guardians != null)
            {
                // Remove existing guardians (or handle update intelligently)
                _context.Guardians.RemoveRange ( existingStudent.Guardians );

                // Add new guardians from view model
                foreach (var guardian in viewModel.Guardians)
                {
                    guardian.StudentId = existingStudent.Id; // Ensure FK is set
                    _context.Guardians.Add ( guardian );
                }
            }

            return await _context.SaveChangesAsync () > 0;
        }

        public async Task<bool> DeleteStudentAsync ( int id )
        {
            var student = await _context.Students
                .Include ( s => s.StudentAttendance )
                .Include ( s => s.StudentClassHistories )
                .Include (s => s.Marks)
                .FirstOrDefaultAsync ( s => s.Id == id );

            if (student == null) return false;

            // Remove related records
            _context.StudentAttendances.RemoveRange ( student.StudentAttendance );
            _context.StudentClassHistories.RemoveRange ( student.StudentClassHistories );

            // Remove student
            _context.Students.Remove ( student );

            return await _context.SaveChangesAsync () > 0;
        }

        public async Task<Student> GetStudentByEmailAsync ( string email )
        {
            return await _context.Students.FirstOrDefaultAsync ( e => e.Email == email );
        }

        public async Task PromoteStudentAsync ( int studentId, int newClassId )
        {
            var student = await _context.Students.FindAsync ( studentId );
            if (student != null)
            {
                student.ClassId = newClassId;
                await _context.SaveChangesAsync ();
            }
        }

        public async Task<bool> ChangePasswordAsync ( int studentId, string newHashedPassword )
        {
            var student = await _context.Students.FindAsync ( studentId );
            if (student == null) return false;

            student.PasswordHash = newHashedPassword;
            await _context.SaveChangesAsync ();
            return true;
        }

        public async Task RegisterStudentWithGuardiansAsync ( Student student, List<Guardian> guardians )
        {
            await _context.Students.AddAsync ( student );
            await _context.SaveChangesAsync ();

            foreach (var guardian in guardians)
            {
                guardian.StudentId = student.Id;
            }

            await _context.Guardians.AddRangeAsync ( guardians );
            await _context.SaveChangesAsync ();
        }

        public async Task<Student> GetStudentByStudentIdAsync ( int studentId )
        {
            return await _context.Students.FirstOrDefaultAsync ( s => s.StudentId == studentId );
        }

        // PDF implementation
        public async Task<StudentGuardianViewModel> GetStudentWithGuardiansAsync ( int id )
        {
            var student = await _context.Students
                .Include ( s => s.Class )
                .FirstOrDefaultAsync ( s => s.Id == id );

            var guardians = await _context.Guardians
                .Where ( g => g.StudentId == id )
                .ToListAsync ();

            if (student == null)
                throw new ArgumentException ( "Student not found" );

            return new StudentGuardianViewModel
            {
                Student = student,
                Guardians = guardians
            };
        }
    }
}
