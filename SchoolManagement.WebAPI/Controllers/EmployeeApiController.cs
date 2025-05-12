using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        // Constructor: Dependency Injection of IEmployeeService.
        public EmployeeApiController ( IEmployeeService employeeService )
        {
            _employeeService = employeeService;
        }

        // GET: api/employeeapi
        // Returns a list of all employees.
        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var employees = await _employeeService.GetAllEmployeeAsync ();
            return Ok ( employees );
        }

        // GET: api/employeeapi/{id}
        // Retrieves a single employee by ID.
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var employee = await _employeeService.GetEmployeeByIdAsync ( id );
            if (employee == null)
                return NotFound ();

            return Ok ( employee );
        }

        // POST: api/employeeapi
        // Creates a new employee.
        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] Employee employee )
        {
            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            await _employeeService.RegisterEmployeeAsync ( employee );
            return CreatedAtAction ( nameof ( GetById ), new { id = employee.Id }, employee );
        }

        // PUT: api/employeeapi/{id}
        // Updates an existing employee.
        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] Employee employee )
        {
            if (id != employee.Id)
                return BadRequest ( "ID Mismatch" );

            await _employeeService.UpdateEmployeeAsync ( employee );
            return NoContent ();
        }

        // DELETE: api/employeeapi/{id}
        // Deletes an employee by ID.
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var existingEmployee = await _employeeService.GetEmployeeByIdAsync ( id );
            if (existingEmployee == null)
                return NotFound ();

            await _employeeService.DeleteEmployeeAsync ( id );
            return NoContent ();
        }
    }
}
