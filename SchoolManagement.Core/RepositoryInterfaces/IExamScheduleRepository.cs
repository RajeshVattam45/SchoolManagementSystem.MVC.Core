using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IExamScheduleRepository
    {
        Task<IEnumerable<ExamSchedule>> GetAllAsync ( );
        Task<ExamSchedule> GetByIdAsync ( int id );
        Task AddAsync ( ExamSchedule schedule );
        void Update ( ExamSchedule schedule );
        void Delete ( ExamSchedule schedule );
        Task SaveChangesAsync ( );
        Task AddRangeAsync ( List<ExamSchedule> schedules );
        Task<Exam> GetExamWithTypeAsync ( int examId );
        Task<IEnumerable<ExamSchedule>> GetByClassAndDateAsync ( int classId, DateTime examDate );
    }
}
