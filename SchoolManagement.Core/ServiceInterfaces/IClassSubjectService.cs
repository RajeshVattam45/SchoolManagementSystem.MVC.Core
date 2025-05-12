using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IClassSubjectService
    {
        Task BulkAssignSubjectsAsync ( Dictionary<int, List<int>> classSubjectMap );
        Task<List<ClassSubject>> GetAssignedSubjectsAsync ( );
        Task RemoveSubjectsAsync ( int classId, List<int> subjectIds );
        Task<BulkAssignViewModel> GetBulkAssignDataAsync ( );
        Task BulkAssignSubjectsAsync ( int classId, List<int> selectedSubjects );
        Task<List<AssignedSubjectsViewModel>> AssignedSubjectsAsync ( );
        Task<List<ClassSubject>> GetAssignedSubjectsByClassIdAsync ( int classId );
    }
}
