using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.Application.Services
{
    public class FeePaymentService : IFeePaymentService
    {
        private readonly IFeePaymentRepository _repository;

        public FeePaymentService ( IFeePaymentRepository repository )
        {
            _repository = repository;
        }

        public async Task AddFeePayment ( FeePayment feePayment )
        {
            await _repository.AddFeePaymentAsync ( feePayment );
        }

        public async Task DeleteFeePayment ( int id )
        {
            await _repository.DeleteFeePayment ( id );
        }

        public async Task<List<FeePayment>> GetAllFeePayment ( )
        {
            return await _repository.GetAllFeePaymentAsync ();
        }

        public async Task<FeePayment> GetFeePaymentsById ( int id )
        {
            return await _repository.GetFeePaymentByIdAsync ( id );
        }

        public async Task UpdateFeePayment ( FeePayment feePayment )
        {
            await _repository.UpdateFeePaymentAsync ( feePayment );
        }
    }
}
