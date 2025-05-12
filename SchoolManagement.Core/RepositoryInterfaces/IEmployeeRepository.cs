using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeeAsync ( );
        Task<Employee> GetEmployeeByIdAsync ( int id );
        Task<Employee> GetEmployeeByEmailAsync ( string email );
        Task AddEmployeeAsync ( Employee employee );
        Task UpdateEmployeeAsync ( Employee employee );
        Task DeleteEmployeeAsync ( int id );
    }
}
