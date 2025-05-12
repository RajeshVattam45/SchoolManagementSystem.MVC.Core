using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IEmployeeAttendanceRepository
    {
        Task<List<EmployeeAttendance>> GetAllEmployeeAttaendanceAsync ( );
        Task<EmployeeAttendance> GetEmployeeAttendanceByIdAsync ( int id );
        Task AddEmployeeAttandanceAsync ( EmployeeAttendance employeeAttendance );
        Task DeleteEmployeeAttendanceByIdAsync ( int id );
        Task EditStudentAttandenceAsync ( EmployeeAttendance employeeAttendance );

    }
}
