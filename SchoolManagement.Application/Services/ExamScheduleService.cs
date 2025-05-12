using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.Infrastructure.Repositories;

namespace SchoolManagement.Application.Services
{
    public class ExamScheduleService : IExamScheduleService
    {
        private readonly IExamScheduleRepository _repository;

        public ExamScheduleService ( IExamScheduleRepository repository )
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ExamSchedule>> GetExamSchedulesAsync ( )
        {
            return await _repository.GetAllAsync ();
        }

        public async Task<ExamSchedule> GetExamScheduleByIdAsync ( int id )
        {
            return await _repository.GetByIdAsync ( id );
        }

        public async Task CreateExamScheduleAsync ( ExamSchedule schedule )
        {
            await _repository.AddAsync ( schedule );
            await _repository.SaveChangesAsync ();
        }

        public async Task UpdateExamScheduleAsync ( ExamSchedule schedule )
        {
            _repository.Update ( schedule );
            await _repository.SaveChangesAsync ();
        }

        public async Task DeleteExamScheduleAsync ( int id )
        {
            var schedule = await _repository.GetByIdAsync ( id );
            if (schedule != null)
            {
                _repository.Delete ( schedule );
                await _repository.SaveChangesAsync ();
            }
        }
    }
}
