using Xunit;
using Moq;
using SchoolManagement.WebAPI.Controllers;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace SchoolManagement.Tests
{
    public class ClassControllerTests
    {
        private readonly Mock<IClassService> _classServiceMock;
        private readonly Mock<IStudentClassHistoryService> _historyServiceMock;
        private readonly ClassController _controller;
        private readonly ITestOutputHelper _output;

        public ClassControllerTests ( ITestOutputHelper output )
        {
            _classServiceMock = new Mock<IClassService> ();
            _historyServiceMock = new Mock<IStudentClassHistoryService> ();
            _controller = new ClassController ( _classServiceMock.Object, _historyServiceMock.Object );
            _output = output;
        }

        [Fact]
        public void GetAllClasses_ReturnsOkWithClasses ( )
        {
            var classList = new List<Class>
            {
                new() { Id = 1, ClassId = 1, ClassName = "First", TotalFee = 1000 }
            };

            _classServiceMock.Setup ( s => s.GetAllClasses () ).Returns ( classList );

            var result = _controller.GetAllClasses ();

            var okResult = Assert.IsType<OkObjectResult> ( result );
            var classes = Assert.IsAssignableFrom<IEnumerable<Class>> ( okResult.Value );
            Assert.Single ( classes );

            _output.WriteLine ( "GetAllClasses test passed." );
        }

        [Fact]
        public void GetClassById_ReturnsClass_WhenExists ( )
        {
            var cls = new Class { Id = 1, ClassId = 1, ClassName = "First" };
            _classServiceMock.Setup ( s => s.GetClassById ( 1 ) ).Returns ( cls );

            var result = _controller.GetClassById ( 1 );

            var okResult = Assert.IsType<OkObjectResult> ( result );
            var returnedClass = Assert.IsType<Class> ( okResult.Value );
            Assert.Equal ( 1, returnedClass.Id );

            _output.WriteLine ( "GetClassById (found) test passed." );
        }

        [Fact]
        public void GetClassById_ReturnsNotFound_WhenNotExists ( )
        {
            _classServiceMock.Setup ( s => s.GetClassById ( 99 ) ).Returns ( (Class)null );

            var result = _controller.GetClassById ( 99 );

            var notFoundResult = Assert.IsType<NotFoundObjectResult> ( result );
            Assert.Equal ( "Class with ID 99 not found.", notFoundResult.Value );

            _output.WriteLine ( "GetClassById (not found) test passed." );
        }

        [Fact]
        public void AddClass_ReturnsCreated_WhenValid ( )
        {
            var cls = new Class { Id = 12, ClassId = 12, ClassName = "Second", TotalFee = 2000 };

            var result = _controller.AddClass ( cls );

            var createdAtAction = Assert.IsType<CreatedAtActionResult> ( result );
            var returnedClass = Assert.IsType<Class> ( createdAtAction.Value );
            Assert.Equal ( "Second", returnedClass.ClassName );

            _output.WriteLine ( "AddClass (valid) test passed." );
        }

        [Fact]
        public void AddClass_ReturnsBadRequest_WhenNull ( )
        {
            var result = _controller.AddClass ( null );

            var badRequest = Assert.IsType<BadRequestObjectResult> ( result );
            Assert.Equal ( "Class data is required.", badRequest.Value );

            _output.WriteLine ( "AddClass (null) test passed." );
        }

        [Fact]
        public void UpdateClass_ReturnsNoContent_WhenExists ( )
        {
            var cls = new Class { Id = 12, ClassId = 12, ClassName = "Third" };

            _classServiceMock.Setup ( s => s.GetClassById ( 12 ) ).Returns ( cls );

            var result = _controller.UpdateClass ( 12, cls );

            Assert.IsType<NoContentResult> ( result );

            _output.WriteLine ( "UpdateClass (existing) test passed." );
        }

        [Fact]
        public void UpdateClass_ReturnsNotFound_WhenNotExists ( )
        {
            _classServiceMock.Setup ( s => s.GetClassById ( 5 ) ).Returns ( (Class)null );

            var result = _controller.UpdateClass ( 5, new Class { Id = 5 } );

            var notFound = Assert.IsType<NotFoundObjectResult> ( result );
            Assert.Equal ( "Class with ID 5 not found.", notFound.Value );

            _output.WriteLine ( "UpdateClass (not found) test passed." );
        }

        [Fact]
        public void DeleteClass_ReturnsNoContent_WhenExists ( )
        {
            var cls = new Class { Id = 12 };

            _classServiceMock.Setup ( s => s.GetClassById ( 12 ) ).Returns ( cls );

            var result = _controller.DeleteClass ( 12 );

            Assert.IsType<NoContentResult> ( result );

            _output.WriteLine ( "DeleteClass (existing) test passed." );
        }

        [Fact]
        public void DeleteClass_ReturnsNotFound_WhenNotExists ( )
        {
            _classServiceMock.Setup ( s => s.GetClassById ( 999 ) ).Returns ( (Class)null );

            var result = _controller.DeleteClass ( 999 );

            var notFound = Assert.IsType<NotFoundObjectResult> ( result );
            Assert.Equal ( "Class with ID 999 not found.", notFound.Value );

            _output.WriteLine ( "DeleteClass (not found) test passed." );
        }
    }
}
