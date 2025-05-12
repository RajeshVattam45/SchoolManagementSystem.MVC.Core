using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IEmployeeAttendanceService
    {
        Task<List<EmployeeAttendance>> GetAttendanceList ( );
        Task<EmployeeAttendance> GetAttendanceById ( int id );
        Task AddEmployeeAttandance ( EmployeeAttendance employeeAttendance );
        Task EditEmployeeAttandance ( EmployeeAttendance employeeAttendance );
        Task DeleteEmployeeAttandance ( int id );
    }
}
