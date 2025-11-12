using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IMarksService
    {
        Task<Marks> RegisterMarksAsync ( Marks marks );
        Task<IEnumerable<MarkDto>> GetStudentMarksAsync ( int studentId );
        Task<IEnumerable<MarksDetailsDto>> GetAllMarksDetailsAsync ( );
        Task<bool> CheckIfMarksExistAsync ( int studentId, int examId, int subjectId, int classId );
        Task<Marks?> GetMarksByIdAsync ( int id );
        Task<Marks?> UpdateMarksAsync ( Marks marks );
        Task<bool> DeleteMarksAsync ( int id );
        Task<MarksEditDto?> GetMarksEditByIdAsync ( int id );
        Task<byte[]> GenerateStudentMarksPdfAsync ( int studentId, string examTypeName );
    }
    public class MarkDto
    {
        public int MarkId { get; set; }
        public string ExamType { get; set; }
        public string ExamName { get; set; }
        public string ExamTypeName { get; set; }
        public string SubjectName { get; set; }
        public int MarksObtained { get; set; }
        public int MaxMarks { get; set; }
    }

}
