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
            try
            {
                string password = employee.PasswordHash;

                if (string.IsNullOrWhiteSpace ( password ))
                {
                    throw new ArgumentException ( "Password is required." );
                }

                if (password.Length < 7)
                {
                    throw new ArgumentException ( "Password must be at least 7 characters long." );
                }

                if (!password.Any ( char.IsUpper ))
                {
                    throw new ArgumentException ( "Password must contain at least one uppercase letter." );
                }

                if (!password.Any ( char.IsLower ))
                {
                    throw new ArgumentException ( "Password must contain at least one lowercase letter." );
                }

                if (!password.Any ( ch => !char.IsLetterOrDigit ( ch ) ))
                {
                    throw new ArgumentException ( "Password must contain at least one special character." );
                }

                if (password.Contains ( " " ))
                {
                    throw new ArgumentException ( "Password must not contain spaces." );
                }

                var passwordHasher = new PasswordHasher<Employee> ();
                employee.PasswordHash = passwordHasher.HashPassword ( employee, password );

                employee.Id = 0;
                await _employeeRepository.AddEmployeeAsync ( employee );
            }
            catch (Exception)
            {
                throw;
            }
            
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
