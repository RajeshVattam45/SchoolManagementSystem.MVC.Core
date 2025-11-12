using Xunit;
using Moq;
using SchoolManagement.WebAPI.Controllers;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SchoolManagement.Tests
{
    public class EmployeeApiControllerTests
    {
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly EmployeeApiController _controller;
        private readonly ITestOutputHelper _output;

        public EmployeeApiControllerTests ( ITestOutputHelper output )
        {
            _employeeServiceMock = new Mock<IEmployeeService> ();
            _controller = new EmployeeApiController ( _employeeServiceMock.Object );
            _output = output;
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithEmployees ( )
        {
            var employees = new List<Employee> {
                new Employee { Id = 1, FirstName = "John Doe", Email = "john@example.com" }
            };
            _employeeServiceMock.Setup ( s => s.GetAllEmployeeAsync () ).ReturnsAsync ( employees );

            var result = await _controller.GetAll ();

            var ok = Assert.IsType<OkObjectResult> ( result );
            var data = Assert.IsAssignableFrom<IEnumerable<Employee>> ( ok.Value );
            Assert.Single ( data );
            _output.WriteLine ( "✅ GetAll test passed." );
        }

        [Fact]
        public async Task GetById_ReturnsEmployee_WhenFound ( )
        {
            var emp = new Employee { Id = 2, FirstName = "Jane Smith" };
            _employeeServiceMock.Setup ( s => s.GetEmployeeByIdAsync ( 2 ) ).ReturnsAsync ( emp );

            var result = await _controller.GetById ( 2 );

            var ok = Assert.IsType<OkObjectResult> ( result );
            var returned = Assert.IsType<Employee> ( ok.Value );
            Assert.Equal ( 2, returned.Id );
            _output.WriteLine ( "✅ GetById (found) test passed." );
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing ( )
        {
            _employeeServiceMock.Setup ( s => s.GetEmployeeByIdAsync ( 99 ) ).ReturnsAsync ( (Employee)null );

            var result = await _controller.GetById ( 99 );

            var notFound = Assert.IsType<NotFoundResult> ( result );
            _output.WriteLine ( "✅ GetById (not found) test passed." );
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenValid ( )
        {
            var newEmp = new Employee { Id = 10, FirstName = "New Emp", Email = "new@emp.com" };

            var result = await _controller.Create ( newEmp );

            var created = Assert.IsType<CreatedAtActionResult> ( result );
            var returned = Assert.IsType<Employee> ( created.Value );
            Assert.Equal ( 10, returned.Id );
            _output.WriteLine ( "✅ Create (valid) test passed." );
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelInvalid ( )
        {
            _controller.ModelState.AddModelError ( "Email", "Required" );

            var result = await _controller.Create ( new Employee () );

            var bad = Assert.IsType<BadRequestObjectResult> ( result );
            _output.WriteLine ( "✅ Create (invalid model) test passed." );
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenValid ( )
        {
            var emp = new Employee { Id = 5, FirstName = "Updated" };

            var result = await _controller.Update ( 5, emp );

            Assert.IsType<NoContentResult> ( result );
            _output.WriteLine ( "✅ Update (valid) test passed." );
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch ( )
        {
            var emp = new Employee { Id = 6, FirstName = "Mismatch" };

            var result = await _controller.Update ( 7, emp );

            var bad = Assert.IsType<BadRequestObjectResult> ( result );
            Assert.Equal ( "ID Mismatch", bad.Value );
            _output.WriteLine ( "✅ Update (ID mismatch) test passed." );
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenFound ( )
        {
            var emp = new Employee { Id = 8 };
            _employeeServiceMock.Setup ( s => s.GetEmployeeByIdAsync ( 8 ) ).ReturnsAsync ( emp );

            var result = await _controller.Delete ( 8 );

            Assert.IsType<NoContentResult> ( result );
            _output.WriteLine ( "✅ Delete (found) test passed." );
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing ( )
        {
            _employeeServiceMock.Setup ( s => s.GetEmployeeByIdAsync ( 99 ) ).ReturnsAsync ( (Employee)null );

            var result = await _controller.Delete ( 99 );

            Assert.IsType<NotFoundResult> ( result );
            _output.WriteLine ( "✅ Delete (not found) test passed." );
        }
    }
}
