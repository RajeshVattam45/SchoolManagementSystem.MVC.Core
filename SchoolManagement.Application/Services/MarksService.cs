using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.Infrastructure.Repositories;

namespace SchoolManagement.Services
{
    public class MarksService : IMarksService
    {
        private readonly IMarksRepository _repo;
        public MarksService ( IMarksRepository repo )
        {
            _repo = repo;
        }

        // new methods
        public async Task<Marks> RegisterMarksAsync ( Marks marks )
        {
            return await _repo.AddMarksAsync ( marks );
        }

        public async Task<IEnumerable<Marks>> GetStudentMarksAsync ( int studentId )
        {
            return await _repo.GetMarksByStudentIdAsync ( studentId );
        }

        public async Task<IEnumerable<MarksDetailsDto>> GetAllMarksDetailsAsync ( )
        {
            var marks = await _repo.GetAllMarksWithDetailsAsync ();

            return marks
                .Where ( m => m.Student != null && m.Exam != null && m.Subject != null && m.Class != null )
                .Select ( m => new MarksDetailsDto
                {
                    StudentFullName = $"{m.Student.FirstName} {m.Student.LastName}",
                    ClassName = m.Class.ClassName,
                    ClassId = m.ClassId ?? 0,
                    AcademicYear = m.Student.AcademicYear,
                    ExamName = m.Exam.ExamName,
                    ExamTypeName = m.Exam.ExamType?.ExamTypeName ?? "N/A",
                    SubjectName = m.Subject.SubjectName,
                    MarksObtained = m.MarksObtained,
                    MaxMarks = m.MaxMarks,
                    Result = m.MarksObtained >= 40 ? "Pass" : "Fail"
                } )
                .ToList ();
        }

        public async Task<bool> CheckIfMarksExistAsync ( int studentId, int examId, int subjectId, int classId )
        {
            return await _repo.CheckIfMarksExistAsync ( studentId, examId, subjectId, classId );
        }
    }
}
