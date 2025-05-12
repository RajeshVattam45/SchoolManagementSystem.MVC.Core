using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.Application.Services
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        public readonly IEmployeeAttendanceRepository _employeeAttendanceRepository;

        public EmployeeAttendanceService ( IEmployeeAttendanceRepository employeeAttendanceRepository )
        {
            _employeeAttendanceRepository = employeeAttendanceRepository;
        }

        public async Task AddEmployeeAttandance ( EmployeeAttendance employeeAttendance )
        {
            await _employeeAttendanceRepository.AddEmployeeAttandanceAsync ( employeeAttendance );
        }

        public async Task DeleteEmployeeAttandance ( int id )
        {
            await _employeeAttendanceRepository.DeleteEmployeeAttendanceByIdAsync ( id );
        }

        public async Task EditEmployeeAttandance ( EmployeeAttendance employeeAttendance )
        {
            await _employeeAttendanceRepository.EditStudentAttandenceAsync ( employeeAttendance );
        }

        public async Task<EmployeeAttendance> GetAttendanceById ( int id )
        {
            return await _employeeAttendanceRepository.GetEmployeeAttendanceByIdAsync ( id );
        }

        public async Task<List<EmployeeAttendance>> GetAttendanceList ( )
        {
            return await _employeeAttendanceRepository.GetAllEmployeeAttaendanceAsync ();
        }
    }
}
