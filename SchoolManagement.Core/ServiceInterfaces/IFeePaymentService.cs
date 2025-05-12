using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IFeePaymentService
    {
        Task<List<FeePayment>> GetAllFeePayment ( );
        Task<FeePayment> GetFeePaymentsById ( int id );
        Task AddFeePayment ( FeePayment feePayment );
        Task UpdateFeePayment ( FeePayment feePayment );
        Task DeleteFeePayment ( int id );
    }
}
