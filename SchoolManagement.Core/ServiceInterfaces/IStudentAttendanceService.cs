using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;


namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IStudentAttendanceService
    {
        Task<List<StudentAttendance>> GetStudentAttendances ( );
        Task RegisterAttendanceAsync ( int studentId, string status, DateOnly date );
        Task<bool> UpdateAttendanceStatusAsync ( int studentId, string status, DateOnly date );
        Task<List<StudentAttendanceGroupedViewModel>> GetGroupedStudentAttendanceAsync ( );
        Task<List<StudentAttendance>> GetAttendancesByStudentEmailAsync ( string email );
    }
}
