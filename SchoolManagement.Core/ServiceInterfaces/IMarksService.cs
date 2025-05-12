using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IMarksService
    {
        Task<Marks> RegisterMarksAsync ( Marks marks );
        Task<IEnumerable<Marks>> GetStudentMarksAsync ( int studentId );
        Task<IEnumerable<MarksDetailsDto>> GetAllMarksDetailsAsync ( );
        Task<bool> CheckIfMarksExistAsync ( int studentId, int examId, int subjectId, int classId );

    }
}
