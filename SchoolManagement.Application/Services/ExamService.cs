using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;

        public ExamService ( IExamRepository examRepository )
        {
            _examRepository = examRepository;
        }

        public async Task<IEnumerable<Exam>> GetAllExamsAsync ( )
        {
            return await _examRepository.GetAllExamsAsync ();
        }

        public async Task<Exam> GetExamByIdAsync ( int id )
        {
            return await _examRepository.GetExamByIdAsync ( id );
        }

        public async Task AddExamAsync ( Exam exam )
        {
            await _examRepository.AddExamAsync ( exam );
        }

        public async Task UpdateExamAsync ( Exam exam )
        {
            await _examRepository.UpdateExamAsync ( exam );
        }

        public async Task DeleteExamAsync ( int id )
        {
            await _examRepository.DeleteExamAsync ( id );
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByExamIdAsync ( int examId )
        {
            return await _examRepository.GetSubjectsByExamIdAsync ( examId );
        }
    }
}
