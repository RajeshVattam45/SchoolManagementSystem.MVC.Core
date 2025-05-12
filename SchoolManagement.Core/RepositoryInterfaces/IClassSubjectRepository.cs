using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IClassSubjectRepository
    {
        Task BulkAssignSubjectsAsync ( List<ClassSubject> classSubjects );
        Task<List<ClassSubject>> GetAssignedSubjectsAsync ( );
        Task RemoveSubjectsAsync ( int classId, List<int> subjectIds );
        Task<List<ClassSubject>> GetAssignedSubjectsByClassIdAsync ( int classId );
    }
}
