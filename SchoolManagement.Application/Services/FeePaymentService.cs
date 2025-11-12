using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels.FeeViewModel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SchoolManagement.Application.Services
{
    public class FeePaymentService : IFeePaymentService
    {
        private readonly IFeePaymentRepository _repo;

        public FeePaymentService ( IFeePaymentRepository repo )
        {
            _repo = repo;
        }

        private FeePaymentDto MapToDto ( FeePayment fp )
        {
            return new FeePaymentDto
            {
                PaymentId = fp.PaymentId,
                StudentId = fp.StudentId,
                StudentName = $"{fp.Student?.FirstName} {fp.Student?.LastName}",
                ClassId = fp.ClassId,
                ClassName = fp.Class?.ClassName,
                FeeType = fp.FeeType,
                AmountPaid = fp.AmountPaid,
                PaymentDate = fp.PaymentDate
            };
        }

        public async Task<List<FeePaymentDto>> GetAllPaymentsAsync ( )
        {
            var payments = await _repo.GetAllAsync ();
            return payments.Select ( MapToDto ).ToList ();
        }

        public async Task<FeePayment?> GetPaymentByIdAsync ( int id )
        {
            return await _repo.GetByIdAsync ( id );
        }

        public async Task AddPaymentAsync ( FeePayment payment )
        {
            if (payment.StudentId == null)
                throw new ArgumentException ( "StudentId is required." );

            // 1. Get full fee summary including penalty
            var summary = await GetStudentFeeSummaryAsync ( payment.StudentId.Value );
            if (summary == null || summary.CurrentStatus == null)
                throw new InvalidOperationException ( "Student or fee status not found." );

            var status = summary.CurrentStatus;

            var totalFee = status.TotalFee;
            var totalPaid = status.TotalPaid;
            var penalty = status.PenaltyAmount;
            var allowedMaxPayment = totalFee + penalty;

            // 2. Validate payment does not exceed total fee + penalty
            var newTotal = totalPaid + payment.AmountPaid;
            if (newTotal > allowedMaxPayment)
            {
                decimal overpayAmount = newTotal - allowedMaxPayment;
                throw new InvalidOperationException (
                    $"Payment rejected: Attempted to pay {payment.AmountPaid:C}, but student has already paid {totalPaid:C} out of allowed {allowedMaxPayment:C} (including penalty). Overpaying by {overpayAmount:C}." );
            }

            // 3. Set metadata
            payment.PaymentDate = DateTime.UtcNow;

            // 4. Save
            await _repo.AddAsync ( payment );
            await _repo.SaveChangesAsync ();
        }

        public async Task UpdatePaymentAsync ( FeePayment payment )
        {
            _repo.Update ( payment );
            await _repo.SaveChangesAsync ();
        }

        public async Task DeletePaymentAsync ( int id )
        {
            var payment = await _repo.GetByIdAsync ( id );
            if (payment != null)
            {
                _repo.Delete ( payment );
                await _repo.SaveChangesAsync ();
            }
        }

        public async Task<List<FeePayment>> GetPaymentsByStudentIdAsync ( int studentId )
        {
            return await _repo.GetByStudentIdAsync ( studentId );
        }

        public async Task<List<FeePayment>> GetPaymentsByClassIdAsync ( int classId )
        {
            return await _repo.GetByClassIdAsync ( classId );
        }

        public async Task<List<UnpaidStudentDto>> GetUnpaidStudentsByClassAsync ( int classId )
        {
            return await _repo.GetUnpaidStudentsAsync ( classId );
        }

        public async Task<StudentFeeStatusDto?> GetStudentFeeStatusAsync ( int studentId, int? classId = null )
        {
            var payments = await _repo.GetByStudentIdAsync ( studentId );
            if (!payments.Any ())
                return null;

            // Get current class payments
            List<FeePayment> relevantPayments;
            Class? cls;

            if (classId.HasValue)
            {
                relevantPayments = payments.Where ( p => p.ClassId == classId.Value ).ToList ();
                cls = relevantPayments.FirstOrDefault ()?.Class
                      ?? payments.FirstOrDefault ( p => p.ClassId == classId.Value )?.Class;
            }
            else
            {
                var latestClassId = payments
                    .OrderByDescending ( p => p.PaymentDate )
                    .FirstOrDefault ()?.ClassId ?? 0;

                relevantPayments = payments.Where ( p => p.ClassId == latestClassId ).ToList ();
                cls = relevantPayments.FirstOrDefault ()?.Class;
                classId = latestClassId;
            }

            var student = payments.FirstOrDefault ( p => p.ClassId == classId )?.Student
                       ?? payments.First ().Student;

            decimal totalPaid = relevantPayments.Sum ( p => p.AmountPaid );
            decimal totalFee = cls?.TotalFee ?? 0;

            DateTime dueDate = new DateTime ( DateTime.Today.Year, 6, 30 );

            bool isOverdue = DateTime.Today > dueDate && totalPaid < totalFee;
            decimal pendingAmount = totalFee - totalPaid;
            decimal penalty = isOverdue ? pendingAmount * 0.05m : 0;

            return new StudentFeeStatusDto
            {
                StudentId = student?.Id ?? 0,
                StudentName = $"{student?.FirstName} {student?.LastName}",
                Phone = student?.PhoneNumber,
                Email = student?.Email,
                ClassId = cls?.ClassId ?? 0,
                ClassName = cls?.ClassName ?? "Unknown Class",
                TotalFee = totalFee,
                TotalPaid = totalPaid,
                PendingAmount = pendingAmount + penalty,
                DueDate = dueDate,
                IsOverdue = isOverdue,
                PenaltyAmount = penalty
            };
        }

        public async Task<StudentFeeSummaryDto?> GetStudentFeeSummaryAsync ( int studentId )
        {
            var payments = await _repo.GetAllPaymentsByStudentIdAsync ( studentId );
            var student = await _repo.GetStudentByIdAsync ( studentId );

            // Always return an empty summary if student or class is missing
            if (student == null || student.Class == null)
            {
                return new StudentFeeSummaryDto
                {
                    CurrentStatus = null,
                    HistoricalPayments = new List<FeePaymentDto> ()
                };
            }

            var currentClass = student.Class;
            var currentClassId = currentClass.ClassId;

            // Only sum current class payments for status
            decimal currentTotalPaid = payments
                .Where ( p => p.ClassId == currentClassId )
                .Sum ( p => p.AmountPaid );

            decimal totalFee = currentClass.TotalFee ?? 0;
            DateTime dueDate = new DateTime ( DateTime.Today.Year, 6, 30 );
            bool isOverdue = DateTime.Today > dueDate && currentTotalPaid < totalFee;
            decimal penalty = isOverdue ? (totalFee - currentTotalPaid) * 0.05m : 0;

            var status = new StudentFeeStatusDto
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                ClassId = currentClass.ClassId,
                ClassName = currentClass.ClassName,
                TotalFee = totalFee,
                TotalPaid = currentTotalPaid,
                PendingAmount = totalFee - currentTotalPaid + penalty,
                DueDate = dueDate,
                IsOverdue = isOverdue,
                PenaltyAmount = penalty,
                Phone = student.PhoneNumber,
                Email = student.Email
            };

            // Historical records for ALL classes
            var historicalPayments = payments
                .Where ( p => p.Class != null )
                .Select ( p => new FeePaymentDto
                {
                    PaymentId = p.PaymentId,
                    StudentId = p.StudentId,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    ClassId = p.ClassId,
                    ClassName = p.Class?.ClassName ?? "N/A",
                    FeeType = p.FeeType,
                    AmountPaid = p.AmountPaid,
                    PaymentDate = p.PaymentDate
                } )
                .ToList ();

            return new StudentFeeSummaryDto
            {
                CurrentStatus = status,
                HistoricalPayments = historicalPayments
            };
        }

        // PDF Implementation
        public byte[] GenerateStudentPaymentPdf ( StudentPaymentDetailsViewModel model )
        {
            var document = Document.Create ( container =>
            {
                container.Page ( page =>
                {
                    page.Margin ( 30 );
                    page.Size ( PageSizes.A4 );
                    page.PageColor ( Colors.White );
                    page.DefaultTextStyle ( x => x.FontSize ( 12 ) );

                    page.Header ().Text ( $"Fee Payment Summary for {model.Status.StudentName}" )
                        .SemiBold ().FontSize ( 16 ).FontColor ( Colors.Blue.Darken2 );

                    page.Content ().PaddingVertical ( 10 ).Column ( col =>
                    {
                        col.Item ().Text ( $"Class: {model.Status.ClassName}" );
                        col.Item ().Text ( $"Mobile Number: {model.Status.Phone}" );
                        col.Item ().Text ( $"Email: {model.Status.Email}" );
                        col.Item ().Text ( $"Total Fee: {model.Status.TotalFee:C}" );
                        col.Item ().Text ( $"Total Paid: {model.Status.TotalPaid:C}" );
                        col.Item ().Text ( $"Pending Amount: {model.Status.PendingAmount:C}" );
                        col.Item ().Text ( $"Due Date: {model.Status.DueDate:yyyy-MM-dd}" );

                        if (model.Status.IsOverdue)
                        {
                            col.Item ().Text ( $"⚠ Overdue - Penalty: {model.Status.PenaltyAmount:C}" )
                                .FontColor ( Colors.Red.Darken2 ).Bold ();
                        }

                        col.Item ().PaddingVertical ( 10 ).Background ( Colors.Grey.Lighten2 ).Height ( 1 );

                        col.Item ()
                       .PaddingBottom ( 5 )
                       .Text ( t => t.Span ( "Payment History:" ).Bold ().FontSize ( 14 ) );


                        col.Item ().Table ( table =>
                        {
                            table.ColumnsDefinition ( columns =>
                            {
                                columns.RelativeColumn ();
                                columns.RelativeColumn ();
                                columns.RelativeColumn ();
                                columns.RelativeColumn ();
                            } );

                            table.Header ( header =>
                            {
                                header.Cell ().Element ( CellStyle ).Text ( "Date" );
                                header.Cell ().Element ( CellStyle ).Text ( "Fee Type" );
                                header.Cell ().Element ( CellStyle ).Text ( "Amount Paid" );
                                header.Cell ().Element ( CellStyle ).Text ( "Class" );
                            } );

                            foreach (var payment in model.Payments)
                            {
                                table.Cell ().Element ( CellStyle ).Text ( payment.PaymentDate.ToShortDateString () );
                                table.Cell ().Element ( CellStyle ).Text ( payment.FeeType );
                                table.Cell ().Element ( CellStyle ).Text ( payment.AmountPaid.ToString ( "C" ) );
                                table.Cell ().Element ( CellStyle ).Text ( payment.ClassName );
                            }

                            IContainer CellStyle ( IContainer container ) =>
                                container.BorderBottom ( 1 ).BorderColor ( Colors.Grey.Lighten2 ).Padding ( 5 );
                        } );
                    } );

                    page.Footer ().AlignCenter ().Text ( x =>
                    {
                        x.Span ( "Generated on " ).FontSize ( 10 );
                        x.Span ( DateTime.Now.ToString ( "yyyy-MM-dd HH:mm" ) ).SemiBold ();
                    } );
                } );
            } );

            return document.GeneratePdf ();
        }
    }
}
