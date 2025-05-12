using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IStudentAttendanceRepository
    {
        Task<List<StudentAttendance>> GetAllAttendancesAsync ( );
        Task<StudentAttendance> GetAllAttendancesByIdAsync ( int id );
        Task AddStudentAttandanceAsync ( StudentAttendance studentAttendance );
        Task UpdateStudentAttandanceAsynce ( StudentAttendance studentAttendance );
        Task DeleteStudentAttendanceAsync ( int id );
        Task<List<StudentAttendance>> GetAttendancesByStudentIdAsync ( int studentId );
        Task<StudentAttendance> GetAttendanceByStudentIdAndDateAsync ( int studentId, DateOnly date );
        Task<IEnumerable<StudentAttendance>> GetStudentAttendanceByClassId ( int classId );
        Task<List<StudentAttendance>> GetAttendancesByStudentEmailAsync ( string email );
    }
}
