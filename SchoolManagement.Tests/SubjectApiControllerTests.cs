using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SchoolManagement.WebAPI.Controllers;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.Tests
{
    public class SubjectApiControllerTests
    {
        private readonly Mock<ISubjectService> _subjectServiceMock;
        private readonly SubjectApiController _controller;

        public SubjectApiControllerTests()
        {
            _subjectServiceMock = new Mock<ISubjectService>();
            _controller = new SubjectApiController(_subjectServiceMock.Object);
        }

        [Fact]
        public void GetAllSubjects_ReturnsOkWithList()
        {
            var subjects = new List<Subject> {
                new Subject { Id = 1, SubjectName = "Math", EmployeeId = 101 }
            };
            _subjectServiceMock.Setup(s => s.GetAllSubjects()).Returns(subjects);

            var result = _controller.GetAllSubjects();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Subject>>(okResult.Value);
        }

        [Fact]
        public async Task GetSubjectById_ReturnsOk_WhenFound()
        {
            var subject = new Subject { Id = 1, SubjectName = "Science" };
            _subjectServiceMock.Setup(s => s.GetSubjectById(1)).Returns(subject);

            var result = await _controller.GetSubjectById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(subject, okResult.Value);
        }

        [Fact]
        public async Task GetSubjectById_ReturnsNotFound_WhenMissing()
        {
            _subjectServiceMock.Setup(s => s.GetSubjectById(2)).Returns((Subject)null);

            var result = await _controller.GetSubjectById(2);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateSubject_ReturnsCreated_WhenValid()
        {
            var subject = new Subject { Id = 3, SubjectName = "English", EmployeeId = 102 };

            var result = await _controller.CreateSubject(subject);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(subject, created.Value);
        }

        [Fact]
        public async Task CreateSubject_ReturnsBadRequest_WhenInvalid()
        {
            _controller.ModelState.AddModelError("SubjectName", "Required");

            var subject = new Subject();
            var result = await _controller.CreateSubject(subject);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateSubject_ReturnsNoContent_WhenValid()
        {
            var subject = new Subject { Id = 5, SubjectName = "History" };

            _subjectServiceMock.Setup(s => s.GetSubjectById(5)).Returns(subject);

            var result = await _controller.UpdateSubject(5, subject);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateSubject_ReturnsBadRequest_OnIdMismatch()
        {
            var subject = new Subject { Id = 99, SubjectName = "Mismatch" };

            var result = await _controller.UpdateSubject(100, subject);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Subject ID mismatch", badRequest.Value);
        }

        [Fact]
        public async Task UpdateSubject_ReturnsNotFound_WhenNotExist()
        {
            _subjectServiceMock.Setup(s => s.GetSubjectById(999)).Returns((Subject)null);

            var subject = new Subject { Id = 999 };

            var result = await _controller.UpdateSubject(999, subject);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteSubject_ReturnsNoContent_WhenFound()
        {
            var subject = new Subject { Id = 10 };
            _subjectServiceMock.Setup(s => s.GetSubjectById(10)).Returns(subject);

            var result = await _controller.DeleteSubject(10);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteSubject_ReturnsNotFound_WhenMissing()
        {
            _subjectServiceMock.Setup(s => s.GetSubjectById(500)).Returns((Subject)null);

            var result = await _controller.DeleteSubject(500);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
