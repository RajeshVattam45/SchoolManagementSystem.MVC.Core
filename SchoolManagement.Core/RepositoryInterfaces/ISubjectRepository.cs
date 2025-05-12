using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface ISubjectRepository
    {
        IEnumerable<Subject> GetAllSubjects ( );
        Subject GetSubjectById ( int id );
        void AddSubject ( Subject subject );
        void UpdateSubject ( Subject subject );
        void DeleteSubject ( int id );
    }
}
