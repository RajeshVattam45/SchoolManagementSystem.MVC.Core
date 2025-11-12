using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IStudentRepository
    {

        Task RegisterStudentAsync ( Student student );
        Task<List<Student>> GetAllStudentsAsync ( );
        //Task<List<StudentGuardianViewModel>> GetAllStudentsWithGuardiansAsync ( );
        Task<List<Guardian>> GetGuardiansByStudentIdAsync ( int studentId );
        Task<Student> GetStudentByIdAsync ( int id );
        //Task<StudentGuardianViewModel> GetStudentWithGuardiansByIdAsync ( int id );
        Task<bool> UpdateStudentAsync ( int id, StudentGuardianViewModel viewModel );
        Task<bool> DeleteStudentAsync ( int id );
        Task<Student> GetStudentByEmailAsync ( string email );
        Task PromoteStudentAsync ( int studentId, int newClassId );
        Task<bool> ChangePasswordAsync ( int studentId, string newHashedPassword );
        Task RegisterStudentWithGuardiansAsync ( Student student, List<Guardian> guardians );

        Task<Student> GetStudentByStudentIdAsync ( int studentId );
        Task<StudentGuardianViewModel> GetStudentWithGuardiansAsync ( int id );

    }
}
