using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly SchoolDbContext _context;

        public SubjectRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public IEnumerable<Subject> GetAllSubjects ( )
        {
            return _context.Subjects.Include ( s => s.Employee ).ToList ();
        }

        public Subject GetSubjectById ( int id )
        {
            return _context.Subjects
                .Include ( s => s.Employee )
                .FirstOrDefault ( s => s.Id == id );
        }

        public void AddSubject ( Subject subject )
        {
            _context.Subjects.Add ( new Subject
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                EmployeeId = subject.EmployeeId
            } );
            _context.SaveChanges ();
        }

        public void UpdateSubject ( Subject subject )
        {
            var existingSubject = _context.Subjects.Find ( subject.Id );
            if (existingSubject != null)
            {
                existingSubject.SubjectId = subject.SubjectId;
                existingSubject.SubjectName = subject.SubjectName;
                existingSubject.EmployeeId = subject.EmployeeId;
                _context.SaveChanges ();
            }
        }

        public void DeleteSubject ( int id )
        {
            var subject = _context.Subjects
                .Include ( s => s.ClassSubjects )
                .Include ( s => s.Marks )
                .Include ( s => s.Timetables )
                .Include ( s => s.ClassSubjectTeachers )
                .Include ( s => s.ExamSchedules )
                .Include ( s => s.ExamSubjects )
                .FirstOrDefault ( s => s.Id == id );

            if (subject != null)
            {
                // Remove related entities first
                if (subject.ClassSubjects != null)
                    _context.ClassSubjects.RemoveRange ( subject.ClassSubjects );

                if (subject.Marks != null)
                    _context.Marks.RemoveRange ( subject.Marks );

                if (subject.Timetables != null)
                    _context.Timetables.RemoveRange ( subject.Timetables );

                if (subject.ClassSubjectTeachers != null)
                    _context.ClassSubjectTeachers.RemoveRange ( subject.ClassSubjectTeachers );

                if (subject.ExamSchedules != null)
                    _context.ExamSchedules.RemoveRange ( subject.ExamSchedules );

                if (subject.ExamSubjects != null)
                    _context.ExamSubjects.RemoveRange ( subject.ExamSubjects );

                // Finally, remove the subject
                _context.Subjects.Remove ( subject );
                _context.SaveChanges ();
            }
        }

    }
}
