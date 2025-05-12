using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly SchoolDbContext _context;

        /// <summary>
        /// Constructor that injects the database context.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public EmployeeRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new employee record to the database.
        /// </summary>
        /// <param name="employee">The employee entity to be added.</param>
        public async Task AddEmployeeAsync ( Employee employee )
        {
            await _context.Employees.AddAsync ( employee );
            await _context.SaveChangesAsync ();
        }

        /// <summary>
        /// Deletes an employee record by ID, if it exists.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        public async Task DeleteEmployeeAsync ( int id )
        {
            var employee = await _context.Employees
                .Include ( e => e.Subjects )
                .ThenInclude ( s => s.ExamSchedules )
                .Include ( e => e.Subjects )
                .ThenInclude ( s => s.ClassSubjects )
                .Include ( e => e.Subjects )
                .ThenInclude ( s => s.Marks )
                .Include ( e => e.Subjects )
                .ThenInclude ( s => s.Timetables )
                .Include ( e => e.Subjects )
                .ThenInclude ( s => s.ExamSubjects )
                .Include ( e => e.ClassSubjectTeachers )
                .Include ( e => e.employeeAttendances )
                .Include ( e => e.Timetables )
                .FirstOrDefaultAsync ( e => e.Id == id );

            if (employee == null)
                throw new Exception ( "Employee not found." );

            // Delete dependent entities manually
            foreach (var subject in employee.Subjects?.ToList () ?? [])
            {
                _context.ExamSchedules.RemoveRange ( subject.ExamSchedules ?? [] );
                _context.ClassSubjects.RemoveRange ( subject.ClassSubjects ?? [] );
                _context.Marks.RemoveRange ( subject.Marks ?? [] );
                _context.Timetables.RemoveRange ( subject.Timetables ?? [] );
                _context.ExamSubjects.RemoveRange ( subject.ExamSubjects ?? [] );
                _context.Subjects.Remove ( subject );
            }

            _context.ClassSubjectTeachers.RemoveRange ( employee.ClassSubjectTeachers ?? [] );
            _context.EmployeeAttendances.RemoveRange ( employee.employeeAttendances ?? [] );
            _context.Timetables.RemoveRange ( employee.Timetables ?? [] );

            _context.Employees.Remove ( employee );
            await _context.SaveChangesAsync ();
        }


        /// <summary>
        /// Retrieves all employee records from the database.
        /// </summary>
        /// <returns>A list of all employees.</returns>
        public async Task<IEnumerable<Employee>> GetAllEmployeeAsync ( )
        {
            return await _context.Employees.ToListAsync ();
        }

        /// <summary>
        /// Retrieves an employee by their unique ID.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>The matching employee object, or null if not found.</returns>
        public async Task<Employee> GetEmployeeByIdAsync ( int id )
        {
            return await _context.Employees.FindAsync ( id );
        }

        /// <summary>
        /// Retrieves an employee based on their email address.
        /// </summary>
        /// <param name="email">The email address of the employee.</param>
        /// <returns>The matching employee object, or null if not found.</returns>
        public async Task<Employee> GetEmployeeByEmailAsync ( string email )
        {
            return await _context.Employees.FirstOrDefaultAsync ( e => e.Email == email );
        }

        /// <summary>
        /// Updates the details of an existing employee.
        /// </summary>
        /// <param name="employee">The updated employee entity.</param>
        public async Task UpdateEmployeeAsync ( Employee employee )
        {
            _context.Employees.Update ( employee );
            await _context.SaveChangesAsync ();
        }
    }
}
