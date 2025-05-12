using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService ( ISubjectRepository subjectRepository )
        {
            _subjectRepository = subjectRepository;
        }

        public IEnumerable<Subject> GetAllSubjects ( )
        {
            return _subjectRepository.GetAllSubjects ();
        }

        public Subject GetSubjectById ( int id )
        {
            return _subjectRepository.GetSubjectById ( id );
        }

        public void AddSubject ( Subject subject )
        {
            _subjectRepository.AddSubject ( subject );
        }

        public void UpdateSubject ( Subject subject )
        {
            _subjectRepository.UpdateSubject ( subject );
        }

        public void DeleteSubject ( int id )
        {
            _subjectRepository.DeleteSubject ( id );
        }
    }
}
