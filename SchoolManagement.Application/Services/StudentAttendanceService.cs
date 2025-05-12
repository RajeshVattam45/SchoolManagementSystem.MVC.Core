using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;

namespace SchoolManagement.Application.Services
{
    public class StudentAttendanceService : IStudentAttendanceService
    {
        private readonly IStudentAttendanceRepository _studentAttendanceRepository;
        private readonly IStudentService _studentService;

        public StudentAttendanceService ( IStudentAttendanceRepository studentAttendanceRepository, IStudentService studentService )
        {
            _studentAttendanceRepository = studentAttendanceRepository;
            _studentService = studentService;
        }

        public async Task<List<StudentAttendance>> GetStudentAttendances ( )
        {
            return await _studentAttendanceRepository.GetAllAttendancesAsync ();
        }


        public async Task<List<StudentAttendanceGroupedViewModel>> GetGroupedStudentAttendanceAsync ( )
        {
            var studentAttendances = await GetStudentAttendances ();
            var students = await _studentService.GetAllStudentsAsync ();

            // List of unique dates from all attendance records (optional: you can use this to fill empty days)
            var attendanceDates = studentAttendances
                .Select ( a => a.Date )
                .Distinct ()
                .OrderBy ( d => d )
                .ToList ();

            return students.Select ( student => new StudentAttendanceGroupedViewModel
            {
                Id = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                AttendanceRecords = attendanceDates
                    .Select ( date =>
                    {
                        var attendance = studentAttendances
                            .FirstOrDefault ( a => a.StudentId == student.Id && a.Date == date );

                        return new DailyAttendance
                        {
                            Id = attendance?.Id ?? 0,
                            Date = date.ToDateTime ( TimeOnly.MinValue ),
                            Class = attendance?.Class?.ClassName ?? student.Class?.ClassName ?? "Unknown Class",
                            Status = string.Equals ( attendance?.Status, "Present", StringComparison.OrdinalIgnoreCase ) ? "Present" : "Absent"
                        };
                    } )
                    .ToList ()
            } ).ToList ();
        }

        public async Task RegisterAttendanceAsync ( int studentId, string status, DateOnly date )
        {
            var student = await _studentService.GetStudentByIdAsync ( studentId );
            if (student == null)
            {
                throw new Exception ( "Student not found." );
            }

            var attendance = await _studentAttendanceRepository.GetAttendanceByStudentIdAndDateAsync ( studentId, date );

            if (attendance == null)
            {
                attendance = new StudentAttendance
                {
                    StudentId = studentId,
                    Date = date,
                    Status = status,
                    ClassId = student.ClassId
                };
                await _studentAttendanceRepository.AddStudentAttandanceAsync ( attendance );
            }
            else
            {
                attendance.Status = status;
                attendance.ClassId = student.ClassId;
                await _studentAttendanceRepository.UpdateStudentAttandanceAsynce ( attendance );
            }
        }


        public async Task<bool> UpdateAttendanceStatusAsync ( int studentId, string status, DateOnly date )
        {
            var student = await _studentService.GetStudentByIdAsync ( studentId );
            if (student == null)
            {
                throw new Exception ( "Student not found." );
            }

            var attendance = await _studentAttendanceRepository.GetAttendanceByStudentIdAndDateAsync ( studentId, date );

            if (attendance == null)
            {
                attendance = new StudentAttendance
                {
                    StudentId = studentId,
                    Date = date,
                    Status = status,
                    ClassId = student.ClassId
                };
                await _studentAttendanceRepository.AddStudentAttandanceAsync ( attendance );
            }
            else
            {
                attendance.Status = status;
                attendance.ClassId = student.ClassId;
                await _studentAttendanceRepository.UpdateStudentAttandanceAsynce ( attendance );
            }

            return true;
        }

        public async Task<List<StudentAttendance>> GetAttendancesByStudentEmailAsync ( string email )
        {
            return await _studentAttendanceRepository.GetAttendancesByStudentEmailAsync ( email );
        }
    }
}
