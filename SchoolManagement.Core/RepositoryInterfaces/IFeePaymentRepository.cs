using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IFeePaymentRepository
    {
        Task<List<FeePayment>> GetAllFeePaymentAsync ( );
        Task<FeePayment> GetFeePaymentByIdAsync ( int id );
        Task AddFeePaymentAsync ( FeePayment payment );
        Task DeleteFeePayment ( int id );
        Task UpdateFeePaymentAsync ( FeePayment payment );
    }
}
