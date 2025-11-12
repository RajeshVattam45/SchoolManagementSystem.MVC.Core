using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;


namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IExamScheduleService
    {
        Task<IEnumerable<ExamSchedule>> GetExamSchedulesAsync ( );
        Task<ExamSchedule> GetExamScheduleByIdAsync ( int id );
        Task CreateExamScheduleAsync ( ExamSchedule schedule );
        Task UpdateExamScheduleAsync ( ExamSchedule schedule );
        Task DeleteExamScheduleAsync ( int id );
        Task ScheduleMultipleExamsAsync ( ExamScheduleBatchRequest request );
    }
}
