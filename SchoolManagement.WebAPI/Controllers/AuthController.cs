using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.WebAPI.Controllers
{
    [Route ( "api/auth" )]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IStudentService _studentService;
        private readonly IConfiguration _configuration;

        public AuthController ( IEmployeeService employeeService, IStudentService studentService, IConfiguration configuration )
        {
            _employeeService = employeeService;
            _studentService = studentService;
            _configuration = configuration;
        }

        [HttpPost ( "login" )]
        public async Task<IActionResult> Login ( [FromBody] LoginViewModel model )
        {
            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            string role = "";
            string email = "";

            // Employee Login
            var employee = await _employeeService.GetEmployeeByEmailAsync ( model.Email );
            if (employee != null && employee.PasswordHash == model.Password)
            {
                email = employee.Email;
                role = employee.Role;
            }
            else
            {
                // Student Login
                var student = await _studentService.GetStudentByEmailAsync ( model.Email );
                if (student != null)
                {
                    var hasher = new PasswordHasher<Student> ();
                    var result = hasher.VerifyHashedPassword ( student, student.PasswordHash, model.Password );
                    if (result == PasswordVerificationResult.Success)
                    {
                        email = student.Email;
                        role = "Student";
                    }
                }
            }

            if (string.IsNullOrEmpty ( email ))
                return Unauthorized ( new { message = "Invalid credentials." } );

            // Generate Token
            var token = GenerateJwtToken ( email, role );

            return Ok ( new
            {
                token,
                email,
                role
            } );
        }

        private string GenerateJwtToken ( string email, string role )
        {
            var jwtSettings = _configuration.GetSection ( "Jwt" );
            var key = new SymmetricSecurityKey ( Encoding.UTF8.GetBytes ( jwtSettings["Key"] ) );
            var creds = new SigningCredentials ( key, SecurityAlgorithms.HmacSha256 );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken (
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours ( 1 ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler ().WriteToken ( token );
        }

        [HttpPost ( "logout" )]
        public IActionResult Logout ( )
        {
            return Ok ( new { message = "User logged out successfully." } );
        }
    }
}
