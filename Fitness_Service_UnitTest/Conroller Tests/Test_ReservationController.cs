using Fitness_Service_API.Controllers;
using Fitness_Service_API.Entities;
using Fitness_Service_API.Infrastructure;
using Fitness_Service_API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

using Xunit;

namespace Fitness_Service_UnitTest.Controllers
{
    public class ReservationsControllerTests
    {
        private readonly Mock<IReservationService> _mockService;
        private readonly ReservationsController _controller;

        public ReservationsControllerTests()
        {
            // ARRANGE: Global setup
            _mockService = new Mock<IReservationService>();
            _controller = new ReservationsController(_mockService.Object);

            // CRITICAL: Clean static state before EVERY test to prevent pollution
            InMemoryDatabase.Members.Clear();
            InMemoryDatabase.Classes.Clear();
            InMemoryDatabase.Reservations.Clear();
        }

        // =========================================================================
        // CREATE RESERVATION TESTS (POST)
        // =========================================================================

        [Fact]
        public void CreateReservation_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var member = new Member { Id = Guid.NewGuid(), Name = "John Doe" };
            var fitnessClass = new FitnessClass { Id = Guid.NewGuid(), Name = "Pilates" };
            var expectedReservation = new Reservation { Id = Guid.NewGuid(), MemberId = member.Id, FitnessClassId = fitnessClass.Id };

            // Add to "DB" so the controller can find them
            InMemoryDatabase.Members.Add(member);
            InMemoryDatabase.Classes.Add(fitnessClass);

            _mockService.Setup(s => s.CreateReservation(member, fitnessClass))
                        .Returns(expectedReservation);

            // Act
            var result = _controller.CreateReservation(member.Id, fitnessClass.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReservation = Assert.IsType<Reservation>(okResult.Value);
            Assert.Equal(expectedReservation.Id, returnedReservation.Id);
        }

        [Fact]
        public void CreateReservation_ReturnsBadRequest_WhenMemberDoesNotExist()
        {
            // Arrange
            var fitnessClass = new FitnessClass { Id = Guid.NewGuid() };
            InMemoryDatabase.Classes.Add(fitnessClass); // Class exists, Member does not

            // Act
            var result = _controller.CreateReservation(Guid.NewGuid(), fitnessClass.Id);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid member or class.", badRequest.Value);
        }

        [Fact]
        public void CreateReservation_ReturnsBadRequest_WhenClassDoesNotExist()
        {
            // Arrange
            var member = new Member { Id = Guid.NewGuid() };
            InMemoryDatabase.Members.Add(member); // Member exists, Class does not

            // Act
            var result = _controller.CreateReservation(member.Id, Guid.NewGuid());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid member or class.", badRequest.Value);
        }

        [Fact]
        public void CreateReservation_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var member = new Member { Id = Guid.NewGuid() };
            var fitnessClass = new FitnessClass { Id = Guid.NewGuid() };

            InMemoryDatabase.Members.Add(member);
            InMemoryDatabase.Classes.Add(fitnessClass);

            // Simulate business logic failure (e.g., class full)
            var errorMessage = "Class capacity exceeded";
            _mockService.Setup(s => s.CreateReservation(member, fitnessClass))
                        .Throws(new InvalidOperationException(errorMessage));

            // Act
            var result = _controller.CreateReservation(member.Id, fitnessClass.Id);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequest.Value);
        }

        // =========================================================================
        // GET ALL TESTS (GET)
        // =========================================================================

        [Fact]
        public void GetAll_ReturnsAllReservations()
        {
            // Arrange
            var res1 = new Reservation { Id = Guid.NewGuid() };
            var res2 = new Reservation { Id = Guid.NewGuid() };
            InMemoryDatabase.Reservations.Add(res1);
            InMemoryDatabase.Reservations.Add(res2);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<Reservation>>(okResult.Value);
            Assert.Equal(2, list.Count);
        }

        // =========================================================================
        // GET BY ID TESTS (GET {id})
        // =========================================================================

        [Fact]
        public void GetById_ReturnsOk_WhenReservationExists()
        {
            // Arrange
            var reservation = new Reservation { Id = Guid.NewGuid() };
            InMemoryDatabase.Reservations.Add(reservation);

            // Act
            var result = _controller.GetById(reservation.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reservation, okResult.Value);
        }

        [Fact]
        public void GetById_ReturnsNotFound_WhenReservationDoesNotExist()
        {
            // Act
            var result = _controller.GetById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // =========================================================================
        // DELETE TESTS (DELETE {id})
        // =========================================================================

        [Fact]
        public void Delete_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.CancelReservation(id)); // Normal void return

            // Act
            var result = _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenServiceThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid();
            // Simulate "Reservation not found" error from service
            _mockService.Setup(s => s.CancelReservation(id))
                        .Throws(new InvalidOperationException("Reservation not found"));

            // Act
            var result = _controller.Delete(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Reservation not found", notFoundResult.Value);
        }
    }
}