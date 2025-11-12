using Microsoft.AspNetCore.Identity;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Infrastructure.Repositories;

namespace SchoolManagement.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService ( IEmployeeRepository employeeRepository )
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeeAsync ( )
        {
            return await _employeeRepository.GetAllEmployeeAsync ();
        }

        public async Task<Employee> GetEmployeeByIdAsync ( int id )
        {
            return await _employeeRepository.GetEmployeeByIdAsync ( id );
        }

        public async Task<Employee> GetEmployeeByEmailAsync ( string email )
        {
            return await _employeeRepository.GetEmployeeByEmailAsync ( email );
        }

        public async Task RegisterEmployeeAsync ( Employee employee )
        {
                employee.Id = 0;
                await _employeeRepository.AddEmployeeAsync ( employee );            
        }

        public async Task UpdateEmployeeAsync ( Employee employee )
        {
            await _employeeRepository.UpdateEmployeeAsync ( employee );
        }

        public async Task DeleteEmployeeAsync ( int id )
        {
            await _employeeRepository.DeleteEmployeeAsync ( id );
        }
    }
}
