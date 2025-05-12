using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IStudentService
    {

        Task RegisterStudentAsync ( Student student );
        Task<List<Student>> GetAllStudentsAsync ( );
        Task<List<Guardian>> GetGuardiansByStudentIdAsync ( int studentId );
        Task<Student> GetStudentByIdAsync ( int id );
        Task<bool> UpdateStudentAsync ( int id, StudentGuardianViewModel viewModel );
        Task<bool> DeleteStudentAsync ( int id );
        Task<Student> GetStudentByEmailAsync ( string email );
        Task PromoteStudentAsync ( int studentId, int newClassId );
        Task<bool> ChangePasswordAsync ( int studentId, string oldPassword, string newPassword );
        Task<byte[]> GenerateStudentDetailsPdfAsync ( int studentId );
        Task RegisterStudentWithGuardiansAsync ( StudentGuardianViewModel model );
        Task<Student> GetStudentByStudentIdAsync ( int studentId );
        void ValidateStudentAcademicPlacement ( Student student );

    }
}
