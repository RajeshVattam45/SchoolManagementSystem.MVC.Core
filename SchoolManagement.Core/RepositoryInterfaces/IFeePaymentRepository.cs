using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IFeePaymentRepository
    {
        Task<List<FeePayment>> GetAllAsync ( );
        Task<FeePayment?> GetByIdAsync ( int id );
        Task AddAsync ( FeePayment entity );
        void Update ( FeePayment entity );
        void Delete ( FeePayment entity );
        Task SaveChangesAsync ( );

        Task<List<FeePayment>> GetByStudentIdAsync ( int studentId );
        Task<List<FeePayment>> GetByClassIdAsync ( int classId );
        Task<List<UnpaidStudentDto>> GetUnpaidStudentsAsync ( int classId );
        public Task<decimal> GetPaidAmountByStudentIdAsync ( int studentId );
        public Task<Student?> GetStudentByIdAsync ( int studentId );
        Task<List<FeePayment>> GetAllPaymentsByStudentIdAsync ( int studentId );

    }

    public class UnpaidStudentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public decimal TotalFee { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal PendingAmount => TotalFee - AmountPaid;
        public bool HasPaid { get; set; }
    }
}
