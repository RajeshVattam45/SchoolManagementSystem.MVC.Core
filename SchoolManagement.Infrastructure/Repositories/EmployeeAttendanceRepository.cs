using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class EmployeeAttendanceRepository : IEmployeeAttendanceRepository
    {
        public readonly SchoolDbContext _context;

        /// <summary>
        /// Constructor to inject the application's database context.
        /// </summary>
        public EmployeeAttendanceRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new employee attendance record to the database.
        /// </summary>
        /// <param name="employeeAttendance">The attendance record to be added.</param>
        public async Task AddEmployeeAttandanceAsync ( EmployeeAttendance employeeAttendance )
        {
            _context.EmployeeAttendances.Add ( employeeAttendance );
            await _context.SaveChangesAsync ();
        }

        /// <summary>
        /// Deletes an employee attendance record by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the attendance record to delete.</param>
        public async Task DeleteEmployeeAttendanceByIdAsync ( int id )
        {
            var employeeAttandance = await _context.EmployeeAttendances.FindAsync ( id );
            if (employeeAttandance != null)
            {
                _context.EmployeeAttendances.Remove ( employeeAttandance );
                await _context.SaveChangesAsync ();
            }
        }

        /// <summary>
        /// Updates an existing employee attendance record.
        /// </summary>
        /// <param name="employeeAttendance">The updated attendance object.</param>
        public async Task EditStudentAttandenceAsync ( EmployeeAttendance employeeAttendance )
        {
            _context.EmployeeAttendances.Update ( employeeAttendance );
            await _context.SaveChangesAsync ();
        }

        /// <summary>
        /// Retrieves all employee attendance records, including related employee information.
        /// </summary>
        /// <returns>A list of all employee attendance records.</returns>
        public async Task<List<EmployeeAttendance>> GetAllEmployeeAttaendanceAsync ( )
        {
            return await _context.EmployeeAttendances
                .Include ( ea => ea.Employee )
                .ToListAsync ();
        }

        /// <summary>
        /// Retrieves a single employee attendance record by its ID.
        /// </summary>
        /// <param name="id">The ID of the attendance record.</param>
        /// <returns>The matching EmployeeAttendance object, or null if not found.</returns>
        public Task<EmployeeAttendance> GetEmployeeAttendanceByIdAsync ( int id )
        {
            return _context.EmployeeAttendances
                .Include ( ea => ea.Employee )
                .FirstOrDefaultAsync ( ea => ea.AttendanceId == id );
        }
    }
}
