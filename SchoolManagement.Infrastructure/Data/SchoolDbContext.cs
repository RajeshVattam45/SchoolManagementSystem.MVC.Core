using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;

namespace SchoolManagementSystem.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext ( DbContextOptions<SchoolDbContext> options ) : base ( options )
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<ClassSubjectTeacher> ClassSubjectTeachers { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamSchedule> ExamSchedules { get; set; }
        public DbSet<ExamSubject> ExamSubjects { get; set; }
        public DbSet<StudentFees> StudentFees { get; set; }
        public DbSet<Marks> Marks { get; set; }
        public DbSet<StudentAttendance> StudentAttendances { get; set; }
        public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
        public DbSet<FeePayment> FeePayments { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Holidays> Holidays { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<LibraryBooks> LibraryBooks { get; set; }
        public DbSet<LibraryTransactions> LibraryTransactions { get; set; }
        public DbSet<StudentClassHistory> StudentClassHistories { get; set; }
        public DbSet<ClassSubject> ClassSubjects { get; set; }
        public DbSet<ContactRequest> ContactRequests { get; set; }
        //public DbSet<AcademicYear> AcademicYears { get; set; }

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            modelBuilder.Entity<Class> ()
              .Property ( c => c.TotalFee )
              .HasPrecision ( 18, 2 );

            modelBuilder.Entity<Employee> ()
                .Property ( e => e.EmployeeSalary )
                .HasPrecision ( 18, 2 );

            base.OnModelCreating ( modelBuilder );
        }

        public override async Task<int> SaveChangesAsync ( CancellationToken cancellationToken = default )
        {
            var studentEntries = ChangeTracker.Entries<Student> ()
                .Where ( e => e.State == EntityState.Modified );

            foreach (var entry in studentEntries)
            {
                var originalClassId = (int?)entry.OriginalValues["ClassId"];
                var newClassId = (int?)entry.CurrentValues["ClassId"];

                if (originalClassId.HasValue && originalClassId != newClassId)
                {
                    var history = new StudentClassHistory
                    {
                        StudentId = entry.Entity.Id,
                        FirstName = entry.Entity.FirstName,
                        LastName = entry.Entity.LastName,
                        ClassId = originalClassId,
                        AcademicYear = DateTime.Now.Year.ToString (),
                        EnrollmentDate = DateTime.Now,
                        CompletionDate = DateTime.Now
                    };

                    StudentClassHistories.Add ( history );
                }
            }

            return await base.SaveChangesAsync ( cancellationToken );
        }
    }
}
