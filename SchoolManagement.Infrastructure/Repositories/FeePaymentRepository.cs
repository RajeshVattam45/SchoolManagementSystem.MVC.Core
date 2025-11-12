using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class FeePaymentRepository : IFeePaymentRepository
    {
        private readonly SchoolDbContext _context;

        public FeePaymentRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task<List<FeePayment>> GetAllAsync ( )
        {
            return await _context.FeePayments
                                 .Include ( fp => fp.Student )
                                 .Include ( fp => fp.Class )
                                 .ToListAsync ();
        }

        public async Task<FeePayment?> GetByIdAsync ( int id )
        {
            return await _context.FeePayments
                                 .Include ( fp => fp.Student )
                                 .Include ( fp => fp.Class )
                                 .FirstOrDefaultAsync ( fp => fp.PaymentId == id );
        }

        public async Task AddAsync ( FeePayment entity )
        {
            await _context.FeePayments.AddAsync ( entity );
        }

        public void Update ( FeePayment entity )
        {
            _context.FeePayments.Update ( entity );
        }

        public void Delete ( FeePayment entity )
        {
            _context.FeePayments.Remove ( entity );
        }

        public async Task SaveChangesAsync ( )
        {
            await _context.SaveChangesAsync ();
        }

        public async Task<List<FeePayment>> GetByStudentIdAsync ( int studentId )
        {
            return await _context.FeePayments
                                 .Where ( fp => fp.StudentId == studentId )
                                 .Include ( fp => fp.Class )
                                 .Include ( fp => fp.Student )
                                 .ToListAsync ();
        }

        public async Task<List<FeePayment>> GetByClassIdAsync ( int classId )
        {
            return await _context.FeePayments
                                 .Where ( fp => fp.ClassId == classId )
                                 .Include ( fp => fp.Student )
                                 .ToListAsync ();
        }

        public async Task<List<UnpaidStudentDto>> GetUnpaidStudentsAsync ( int classId )
        {
            var students = await _context.Students
                .Where ( s => s.ClassId == classId )
                .Include ( s => s.Class )
                .Include ( s => s.FeePayments )
                .ToListAsync ();

            return students
                .Select ( s =>
                {
                    decimal totalPaid = s.FeePayments?.Where ( p => p.ClassId == classId ).Sum ( p => p.AmountPaid ) ?? 0;
                    decimal totalFee = s.Class?.TotalFee ?? 0;

                    return new UnpaidStudentDto
                    {
                        StudentId = s.Id,
                        StudentName = s.FirstName + " " + s.LastName,
                        ClassId = s.ClassId ?? 0,
                        ClassName = s.Class?.ClassName ?? "",
                        TotalFee = totalFee,
                        AmountPaid = totalPaid,
                        HasPaid = totalPaid >= totalFee
                    };
                } )
                .Where ( dto => !dto.HasPaid )
                .ToList ();
        }

        public async Task<decimal> GetPaidAmountByStudentIdAsync ( int studentId )
        {
            return await _context.FeePayments
                .Where ( p => p.StudentId == studentId )
                .SumAsync ( p => (decimal?)p.AmountPaid ) ?? 0m;
        }

        public async Task<Student?> GetStudentByIdAsync ( int studentId )
        {
            return await _context.Students
                .Include ( s => s.Class )
                .FirstOrDefaultAsync ( s => s.Id == studentId );
        }

        public async Task<List<FeePayment>> GetAllPaymentsByStudentIdAsync ( int studentId )
        {
            return await _context.FeePayments
                .Where ( fp => fp.StudentId == studentId )
                .Include ( fp => fp.Class )
                .Include ( fp => fp.Student )
                .OrderByDescending ( fp => fp.PaymentDate )
                .ToListAsync ();
        }
    }
}
