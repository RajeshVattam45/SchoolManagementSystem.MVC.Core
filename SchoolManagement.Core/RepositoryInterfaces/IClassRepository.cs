using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IClassRepository
    {
        IEnumerable<Class> GetAllClasses ( );
        Class GetClassById ( int id );
        void AddClass ( Class cls );
        void UpdateClass ( Class cls );
        void DeleteClass ( int id );
    }
}
