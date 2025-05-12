using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class FeePaymentRepository : IFeePaymentRepository
    {
        public readonly SchoolDbContext _context;

        public FeePaymentRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task AddFeePaymentAsync ( FeePayment payment )
        {
            await _context.FeePayments.AddAsync ( payment );
            await _context.SaveChangesAsync ();

        }

        public async Task DeleteFeePayment ( int id )
        {
            var feePayment = await _context.FeePayments.FindAsync ( id );
            if (feePayment != null)
            {
                _context.FeePayments.Remove ( feePayment );
                await _context.SaveChangesAsync ();
            }
        }

        public async Task<List<FeePayment>> GetAllFeePaymentAsync ( )
        {
            return await _context.FeePayments
                .Include ( fp => fp.Student )
                .Include ( fp => fp.Class )
                .ToListAsync ();
        }

        public async Task<FeePayment> GetFeePaymentByIdAsync ( int id )
        {
            return await _context.FeePayments
                .Include ( fp => fp.Student )
                .Include ( fp => fp.Class )
                .FirstOrDefaultAsync ( fp => fp.PaymentId == id );
        }

        public async Task UpdateFeePaymentAsync ( FeePayment payment )
        {
            _context.FeePayments.Update ( payment );
            await _context.SaveChangesAsync ();
        }
    }
}
