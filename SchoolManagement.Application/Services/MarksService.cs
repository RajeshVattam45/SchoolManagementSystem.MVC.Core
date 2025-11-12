using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.Infrastructure.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

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


        //public async Task<IEnumerable<MarkDto>> GetStudentMarksAsync ( int studentId )
        //{
        //    var marks = await _repo.GetMarksByStudentIdAsync ( studentId );

        //    var dtoList = marks.Select ( m => new MarkDto
        //    {
        //        MarkId = m.MarkId,
        //        ExamType = m.Exam?.ExamType.ToString(),
        //        ExamName = m.Exam?.ExamName,
        //        ExamTypeName = m.Exam?.ExamType?.ExamTypeName,
        //        SubjectName = m.Subject?.SubjectName,
        //        MarksObtained = m.MarksObtained.Value,
        //        MaxMarks = m.MaxMarks.Value
        //    } );

        //    return dtoList;
        //}
        public async Task<IEnumerable<MarkDto>> GetStudentMarksAsync ( int studentId )
        {
            var marks = await _repo.GetMarksByStudentIdAsync ( studentId );

            if (!marks.Any ())
                return Enumerable.Empty<MarkDto> ();

            // Get the most recent class based on StudentClassHistory or from Marks' ClassId
            var latestClassId = marks
                .Where ( m => m.ClassId.HasValue )
                .OrderByDescending ( m => m.Exam?.ExamDate ?? DateTime.MinValue )
                .Select ( m => m.ClassId )
                .FirstOrDefault ();

            if (latestClassId == null)
                return Enumerable.Empty<MarkDto> ();

            var dtoList = marks
                .Where ( m => m.ClassId == latestClassId )
                .Select ( m => new MarkDto
                {
                    MarkId = m.MarkId,
                    ExamType = m.Exam?.ExamType.ToString (),
                    ExamName = m.Exam?.ExamName,
                    ExamTypeName = m.Exam?.ExamType?.ExamTypeName,
                    SubjectName = m.Subject?.SubjectName,
                    MarksObtained = m.MarksObtained.Value,
                    MaxMarks = m.MaxMarks.Value
                } );

            return dtoList;
        }

        public async Task<IEnumerable<MarksDetailsDto>> GetAllMarksDetailsAsync ( )
        {
            var marks = await _repo.GetAllMarksWithDetailsAsync ();

            return marks
                .Where ( m => m.Student != null && m.Exam != null && m.Subject != null && m.Class != null )
                .Select ( m => new MarksDetailsDto
                {
                    MarkId = m.MarkId,
                    StudentFullName = $"{m.Student.FirstName} {m.Student.LastName}",
                    ClassName = m.Class.ClassName,
                    ClassId = m.ClassId ?? 0,
                    AcademicYear = m.Student.AcademicYear,
                    ExamName = m.Exam.ExamName,
                    ExamTypeName = m.Exam.ExamType?.ExamTypeName ?? "N/A",
                    SubjectName = m.Subject.SubjectName,
                    MarksObtained = m.MarksObtained.Value,
                    MaxMarks = m.MaxMarks.Value,
                    Result = m.MarksObtained >= 40 ? "Pass" : "Fail"
                } )
                .ToList ();
        }

        public async Task<bool> CheckIfMarksExistAsync ( int studentId, int examId, int subjectId, int classId )
        {
            return await _repo.CheckIfMarksExistAsync ( studentId, examId, subjectId, classId );
        }

        public async Task<Marks?> GetMarksByIdAsync ( int id )
        {
            return await _repo.GetMarksByIdAsync ( id );
        }

        public async Task<Marks?> UpdateMarksAsync ( Marks marks )
        {
            return await _repo.UpdateMarksAsync ( marks );
        }

        public async Task<bool> DeleteMarksAsync ( int id )
        {
            return await _repo.DeleteMarksAsync ( id );
        }

        public async Task<MarksEditDto?> GetMarksEditByIdAsync ( int id )
        {
            var mark = await _repo.GetMarksByIdAsync ( id );
            if (mark == null) return null;

            return new MarksEditDto
            {
                MarkId = mark.MarkId,
                StudentId = mark.StudentId ?? 0,
                ExamId = mark.ExamId ?? 0,
                SubjectId = mark.SubjectId ?? 0,
                ClassId = mark.ClassId ?? 0,
                MarksObtained = mark.MarksObtained.Value,
                MaxMarks = mark.MaxMarks.Value
            };
        }

        public async Task<byte[]> GenerateStudentMarksPdfAsync ( int studentId, string examTypeName )
        {
            var marks = await _repo.GetMarksByStudentIdAsync ( studentId );
            var filtered = marks
                .Where ( m => m.Exam?.ExamType?.ExamTypeName == examTypeName )
                .ToList ();

            if (!filtered.Any ()) return null;

            var studentName = filtered.First ().Student?.FirstName + " " + filtered.First ().Student?.LastName;
            var className = filtered.First ().Class?.ClassName ?? "N/A";

            var document = Document.Create ( container =>
            {
                container.Page ( page =>
                {
                    page.Margin ( 30 );
                    page.Header ().Text ( $"Marks Report - {examTypeName}" ).SemiBold ().FontSize ( 18 ).FontColor ( Colors.Blue.Medium );
                    page.Content ().Column ( col =>
                    {
                        col.Item ().Text ( $"Student: {studentName}" );
                        col.Item ().Text ( $"Class: {className}" );
                        col.Item ().LineHorizontal ( 1 ).LineColor ( Colors.Grey.Lighten2 );

                        col.Item ().Table ( table =>
                        {
                            table.ColumnsDefinition ( columns =>
                            {
                                columns.ConstantColumn ( 100 ); // Subject
                                columns.RelativeColumn ();    // Exam
                                columns.ConstantColumn ( 60 );  // Marks
                                columns.ConstantColumn ( 60 );  // Max
                                columns.ConstantColumn ( 60 );  // %
                                columns.ConstantColumn ( 60 );  // Result
                            } );

                            // Header
                            table.Header ( header =>
                            {
                                header.Cell ().Text ( "Subject" ).Bold ();
                                header.Cell ().Text ( "Exam Name" ).Bold ();
                                header.Cell ().Text ( "Marks" ).Bold ();
                                header.Cell ().Text ( "Max" ).Bold ();
                                header.Cell ().Text ( "%" ).Bold ();
                                header.Cell ().Text ( "Result" ).Bold ();
                            } );

                            // Rows
                            foreach (var m in filtered)
                            {
                                var percent = (double)m.MarksObtained / m.MaxMarks * 100;
                                var result = percent >= 35 ? "Pass" : "Fail";

                                table.Cell ().Text ( m.Subject?.SubjectName ?? "-" );
                                table.Cell ().Text ( m.Exam?.ExamName ?? "-" );
                                table.Cell ().Text ( m.MarksObtained.ToString () );
                                table.Cell ().Text ( m.MaxMarks.ToString () );
                                table.Cell ().Text ( $"{percent:F1}%" );
                                table.Cell ().Text ( result ).FontColor ( result == "Pass" ? Colors.Green.Medium : Colors.Red.Medium );
                            }
                        } );

                        col.Item ().PaddingTop ( 10 );
                        var total = filtered.Sum ( x => x.MarksObtained );
                        var max = filtered.Sum ( x => x.MaxMarks );
                        var overallPercent = max > 0 ? (double)total / max * 100 : 0;
                        var overallResult = filtered.Any ( x => ((double)x.MarksObtained / x.MaxMarks) * 100 < 35 ) ? "Fail" : "Pass";

                        col.Item ().Text ( $"Total: {total} / {max}" ).Bold ();
                        col.Item ().Text ( $"Overall: {overallPercent:F2}% - {overallResult}" )
                            .Bold ().FontColor ( overallResult == "Pass" ? Colors.Green.Medium : Colors.Red.Medium );
                    } );

                    page.Footer ().AlignCenter ().Text ( text =>
                    {
                        text.Span ( "Generated on " ).FontSize ( 10 );
                        text.Span ( DateTime.Now.ToString ( "yyyy-MM-dd HH:mm" ) ).SemiBold ().FontSize ( 10 );
                    } );
                } );
            } );

            return document.GeneratePdf ();
        }
    }
}
