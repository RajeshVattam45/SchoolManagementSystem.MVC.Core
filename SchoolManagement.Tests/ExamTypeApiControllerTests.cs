using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.WebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SchoolManagement.Tests.Controllers
{
    public class ExamTypeApiControllerTests
    {
        private readonly Mock<IExamTypeService> _examTypeServiceMock;
        private readonly ExamTypeApiController _controller;

        public ExamTypeApiControllerTests ( )
        {
            _examTypeServiceMock = new Mock<IExamTypeService> ();
            _controller = new ExamTypeApiController ( _examTypeServiceMock.Object );
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList ( )
        {
            var mockData = new List<ExamType> {
                new ExamType { Id = 1, ExamTypeName = "Midterm" },
                new ExamType { Id = 2, ExamTypeName = "Finals" }
            };

            _examTypeServiceMock.Setup ( s => s.GetAllExamTypesAsync () ).ReturnsAsync ( mockData );

            var result = await _controller.GetAll ();

            var okResult = Assert.IsType<OkObjectResult> ( result );
            var examTypes = Assert.IsAssignableFrom<IEnumerable<ExamType>> ( okResult.Value );

            Assert.NotNull ( examTypes );
            Assert.Equal ( 2, ((List<ExamType>)examTypes).Count );
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound ( )
        {
            var mockExamType = new ExamType { Id = 1, ExamTypeName = "Unit Test" };
            _examTypeServiceMock.Setup ( s => s.GetExamTypeByIdAsync ( 1 ) ).ReturnsAsync ( mockExamType );

            var result = await _controller.GetById ( 1 );

            var okResult = Assert.IsType<OkObjectResult> ( result );
            var examType = Assert.IsType<ExamType> ( okResult.Value );

            Assert.Equal ( mockExamType.Id, examType.Id );
            Assert.Equal ( mockExamType.ExamTypeName, examType.ExamTypeName );
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing ( )
        {
            _examTypeServiceMock.Setup ( s => s.GetExamTypeByIdAsync ( 99 ) ).ReturnsAsync ( (ExamType)null );

            var result = await _controller.GetById ( 99 );

            Assert.IsType<NotFoundResult> ( result );
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenValid ( )
        {
            var newExam = new ExamType { Id = 1, ExamTypeName = "Prelims" };
            _controller.ModelState.Clear ();

            var result = await _controller.Create ( newExam );

            var createdResult = Assert.IsType<CreatedAtActionResult> ( result );
            var returnedExam = Assert.IsType<ExamType> ( createdResult.Value );

            Assert.Equal ( newExam.Id, returnedExam.Id );
            Assert.Equal ( newExam.ExamTypeName, returnedExam.ExamTypeName );
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelInvalid ( )
        {
            _controller.ModelState.AddModelError ( "ExamTypeName", "Required" );

            var result = await _controller.Create ( new ExamType () );

            var badResult = Assert.IsType<BadRequestObjectResult> ( result );

            // ModelState errors come as SerializableError
            var errors = Assert.IsType<SerializableError> ( badResult.Value );

            Assert.True ( errors.ContainsKey ( "ExamTypeName" ), "Expected model error key 'ExamTypeName' not found." );
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenValid ( )
        {
            var updatedExam = new ExamType { Id = 1, ExamTypeName = "Updated Test" };

            var result = await _controller.Update ( 1, updatedExam );

            Assert.IsType<NoContentResult> ( result );
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch ( )
        {
            var updatedExam = new ExamType { Id = 2, ExamTypeName = "Mismatch" };

            var result = await _controller.Update ( 1, updatedExam );

            var badResult = Assert.IsType<BadRequestObjectResult> ( result );
            Assert.Equal ( "ID mismatch", badResult.Value );
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess ( )
        {
            var result = await _controller.Delete ( 1 );

            Assert.IsType<NoContentResult> ( result );
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_OnInvalidOperation ( )
        {
            _examTypeServiceMock.Setup ( s => s.DeleteExamTypeAsync ( It.IsAny<int> () ) )
                .ThrowsAsync ( new InvalidOperationException ( "Cannot delete" ) );

            var result = await _controller.Delete ( 10 );

            var badRequest = Assert.IsType<BadRequestObjectResult> ( result );
            Assert.Contains ( "Cannot delete", badRequest.Value.ToString () );
        }

        [Fact]
        public async Task Delete_ReturnsServerError_OnException ( )
        {
            _examTypeServiceMock.Setup ( s => s.DeleteExamTypeAsync ( It.IsAny<int> () ) )
                .ThrowsAsync ( new Exception ( "Unexpected" ) );

            var result = await _controller.Delete ( 999 );

            var status = Assert.IsType<ObjectResult> ( result );
            Assert.Equal ( 500, status.StatusCode );
        }
    }
}
