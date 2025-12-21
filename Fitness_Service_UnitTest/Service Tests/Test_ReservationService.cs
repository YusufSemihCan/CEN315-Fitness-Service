using Fitness_Service_API.Infrastructure;
using Fitness_Service_API.Services;
using Fitness_Service_API.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fitness_Service_UnitTest;

public class ReservationsControllerTests
{
    private readonly Mock<IReservationService> _reservationServiceMock;
    private readonly ReservationsController _controller;

    public ReservationsControllerTests()
    {
        _reservationServiceMock = new Mock<IReservationService>();
        _controller = new ReservationsController(_reservationServiceMock.Object);

        // Important: clean static DB before every test
        InMemoryDatabase.Members.Clear();
        InMemoryDatabase.Classes.Clear();
        InMemoryDatabase.Reservations.Clear();
    }

    [Fact]
    public void CreateReservation_ReturnsBadRequest_WhenMemberNotFound()
    {
        // Arrange
        var classId = Guid.NewGuid();
        InMemoryDatabase.Classes.Add(new FitnessClass
        {
            Id = classId,
            Capacity = 10
        });

        // Act
        var result = _controller.CreateReservation(Guid.NewGuid(), classId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateReservation_ReturnsBadRequest_WhenClassNotFound()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        InMemoryDatabase.Members.Add(new Member
        {
            Id = memberId
        });

        // Act
        var result = _controller.CreateReservation(memberId, Guid.NewGuid());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateReservation_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var member = new Member { Id = Guid.NewGuid() };
        var fitnessClass = new FitnessClass
        {
            Id = Guid.NewGuid(),
            Capacity = 5
        };

        InMemoryDatabase.Members.Add(member);
        InMemoryDatabase.Classes.Add(fitnessClass);

        var reservation = new Reservation
        {
            MemberId = member.Id,
            FitnessClassId = fitnessClass.Id
        };

        _reservationServiceMock
            .Setup(s => s.CreateReservation(
                It.Is<Member>(m => m.Id == member.Id),
                It.Is<FitnessClass>(c => c.Id == fitnessClass.Id)))
            .Returns(reservation);

        // Act
        var result = _controller.CreateReservation(member.Id, fitnessClass.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(reservation, okResult.Value);
    }

    [Fact]
    public void CreateReservation_ReturnsBadRequest_WhenServiceThrows()
    {
        // Arrange
        var member = new Member { Id = Guid.NewGuid() };
        var fitnessClass = new FitnessClass
        {
            Id = Guid.NewGuid(),
            Capacity = 1
        };

        InMemoryDatabase.Members.Add(member);
        InMemoryDatabase.Classes.Add(fitnessClass);

        _reservationServiceMock
            .Setup(s => s.CreateReservation(
                It.Is<Member>(m => m.Id == member.Id),
                It.Is<FitnessClass>(c => c.Id == fitnessClass.Id)))
            .Throws(new InvalidOperationException("Class full"));

        // Act
        var result = _controller.CreateReservation(member.Id, fitnessClass.Id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetAll_ReturnsOk_WithReservations()
    {
        // Arrange
        InMemoryDatabase.Reservations.Add(new Reservation());

        // Act
        var result = _controller.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<Reservation>>(ok.Value);
        Reservation reservation = Assert.Single(list);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenMissing()
    {
        // Act
        var result = _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetById_ReturnsOk_WhenExists()
    {
        // Arrange
        var reservation = new Reservation { Id = Guid.NewGuid() };
        InMemoryDatabase.Reservations.Add(reservation);

        // Act
        var result = _controller.GetById(reservation.Id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(reservation, ok.Value);
    }

    [Fact]
    public void Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var reservationId = Guid.NewGuid();

        _reservationServiceMock
            .Setup(s => s.CancelReservation(reservationId));

        // Act
        var result = _controller.Delete(reservationId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Delete_ReturnsNotFound_WhenServiceThrows()
    {
        // Arrange
        var reservationId = Guid.NewGuid();

        _reservationServiceMock
            .Setup(s => s.CancelReservation(reservationId))
            .Throws(new InvalidOperationException());

        // Act
        var result = _controller.Delete(reservationId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
