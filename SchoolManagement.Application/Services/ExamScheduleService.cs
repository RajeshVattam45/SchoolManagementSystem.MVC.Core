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

        //public async Task CreateExamScheduleAsync ( ExamSchedule schedule )
        //{
        //    await _repository.AddAsync ( schedule );
        //    await _repository.SaveChangesAsync ();
        //}

        public async Task CreateExamScheduleAsync ( ExamSchedule schedule )
        {
            // Load all schedules (not filtered by date)
            var allSchedules = await _repository.GetAllAsync ();

            // Filter only the ones with same date for time conflict check
            var sameDateSchedules = allSchedules
                .Where ( e => e.ExamDate.Date == schedule.ExamDate.Date )
                .ToList ();

            // Rule 1: Class time conflict on the same date
            var classConflicts = sameDateSchedules
                .Where ( e => e.ClassId == schedule.ClassId )
                .Where ( e =>
                    TimeRangesOverlapWithinTwoHours (
                        e.StartTime, e.EndTime,
                        schedule.StartTime, schedule.EndTime ) )
                .ToList ();

            if (classConflicts.Any ())
                throw new InvalidOperationException ( "Another exam is already scheduled for this class within 2 hours on the same date." );

            // Rule 2: Same exam & subject already exists for this class (across any date)
            var duplicateExam = allSchedules
                .Any ( e => e.ExamId == schedule.ExamId &&
                          e.SubjectId == schedule.SubjectId &&
                          e.ClassId == schedule.ClassId );

            if (duplicateExam)
                throw new InvalidOperationException ( "This subject exam is already scheduled for this class and exam type." );

            // Rule 3: Room conflict (2-hour gap rule, same date)
            var roomConflicts = sameDateSchedules
                .Where ( e => e.RoomNumber == schedule.RoomNumber )
                .Where ( e =>
                    TimeRangesOverlapWithinTwoHours (
                        e.StartTime, e.EndTime,
                        schedule.StartTime, schedule.EndTime ) )
                .ToList ();

            if (roomConflicts.Any ())
                throw new InvalidOperationException ( $"Room '{schedule.RoomNumber}' is already booked within 2 hours of this time." );

            // All checks passed, proceed to insert
            await _repository.AddAsync ( schedule );
            await _repository.SaveChangesAsync ();
        }

        private bool TimeOverlap ( TimeSpan existingStart, TimeSpan existingEnd, TimeSpan newStart, TimeSpan newEnd )
        {
            return newStart < existingEnd && existingStart < newEnd;
        }

        private bool TimeRangesOverlapWithinTwoHours (
     TimeSpan existingStart, TimeSpan existingEnd,
     TimeSpan newStart, TimeSpan newEnd )
        {
            var overlap = newStart < existingEnd && existingStart < newEnd;
            if (!overlap) return false;

            var duration = (existingStart < newStart ? newStart : existingStart) -
                           (existingEnd > newEnd ? newEnd : existingEnd);

            return duration.TotalMinutes <= 120;
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

        public async Task ScheduleMultipleExamsAsync ( ExamScheduleBatchRequest request )
        {
            var schedules = request.Schedules.Select ( e => new ExamSchedule
            {
                SubjectId = e.SubjecId,
                ExamId = e.ExamId,
                ClassId = request.ClassId,
                ExamDate = e.ExamDate,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                RoomNumber = e.RoomNumber
            } ).ToList ();

            await _repository.AddRangeAsync ( schedules );
            await _repository.SaveChangesAsync ();
        }
    }
}
