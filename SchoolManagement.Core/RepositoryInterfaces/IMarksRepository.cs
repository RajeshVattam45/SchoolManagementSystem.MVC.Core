using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IMarksRepository
    {
        Task<IEnumerable<Marks>> GetMarksByStudentIdAsync ( int studentId );
        Task<Marks> AddMarksAsync ( Marks marks );
        Task<IEnumerable<Marks>> GetAllMarksWithDetailsAsync ( );
        Task<bool> CheckIfMarksExistAsync ( int studentId, int examId, int subjectId, int classId );

        Task<Marks?> GetMarksByIdAsync ( int id );
        Task<Marks?> UpdateMarksAsync ( Marks marks );
        Task<bool> DeleteMarksAsync ( int id );
    }
}
