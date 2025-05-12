using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface ISubjectService
    {
        IEnumerable<Subject> GetAllSubjects ( );
        Subject GetSubjectById ( int id );
        void AddSubject ( Subject subject );
        void UpdateSubject ( Subject subject );
        void DeleteSubject ( int id );
    }
}
