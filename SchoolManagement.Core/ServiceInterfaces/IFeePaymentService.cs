using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ViewModels.FeeViewModel;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IFeePaymentService
    {
        Task<List<FeePaymentDto>> GetAllPaymentsAsync ( );
        Task<FeePayment?> GetPaymentByIdAsync ( int id );
        Task AddPaymentAsync ( FeePayment payment );
        Task UpdatePaymentAsync ( FeePayment payment );
        Task DeletePaymentAsync ( int id );
        Task<List<FeePayment>> GetPaymentsByStudentIdAsync ( int studentId );
        Task<List<FeePayment>> GetPaymentsByClassIdAsync ( int classId );
        Task<List<UnpaidStudentDto>> GetUnpaidStudentsByClassAsync ( int classId );
        //Task<StudentFeeStatusDto?> GetStudentFeeStatusAsync ( int studentId );
        Task<StudentFeeStatusDto?> GetStudentFeeStatusAsync ( int studentId, int? classId = null );
        byte[] GenerateStudentPaymentPdf ( StudentPaymentDetailsViewModel model );
        Task<StudentFeeSummaryDto?> GetStudentFeeSummaryAsync ( int studentId );
    }

    public class StudentFeeSummaryDto
    {
        public StudentFeeStatusDto CurrentStatus { get; set; } = new ();
        public List<FeePaymentDto> HistoricalPayments { get; set; } = new ();
    }

    public class FeePaymentDto
    {
        public int PaymentId { get; set; }
        public int? StudentId { get; set; }
        public string? StudentName { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public string FeeType { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class StudentFeeStatusDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public decimal TotalFee { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal PendingAmount { get; set; }

        public DateTime DueDate { get; set; }
        public bool IsOverdue { get; set; }
        public decimal PenaltyAmount { get; set; }
    }

}
