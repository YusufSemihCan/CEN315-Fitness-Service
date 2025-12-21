using Fitness_Service_API.Infrastructure;
using Fitness_Service_API.Services;
using Fitness_Service_API.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fitness_Service_UnitTest.Controllers;

public class ReservationControllerTests
{
    private readonly Mock<IReservationService> _reservationServiceMock;
    private readonly ReservationsController _controller;

    public ReservationControllerTests()
    {
        _reservationServiceMock = new Mock<IReservationService>();
        _controller = new ReservationsController(_reservationServiceMock.Object);

        // IMPORTANT: reset shared static state
        InMemoryDatabase.Members.Clear();
        InMemoryDatabase.Classes.Clear();
        InMemoryDatabase.Reservations.Clear();
    }

    [Fact]
    public void CreateReservation_ValidInput_ReturnsOk()
    {
        // Arrange
        var member = new Member { Id = Guid.NewGuid(), Name = "Test", MembershipType = MembershipType.Standard };
        var fitnessClass = new FitnessClass
        {
            Id = Guid.NewGuid(),
            Name = "Yoga",
            Capacity = 10,
            StartTime = DateTime.Now
        };

        InMemoryDatabase.Members.Add(member);
        InMemoryDatabase.Classes.Add(fitnessClass);

        var reservation = new Reservation { MemberId = member.Id, FitnessClassId = fitnessClass.Id };

        _reservationServiceMock
            .Setup(s => s.CreateReservation(member, fitnessClass))
            .Returns(reservation);

        // Act
        var result = _controller.CreateReservation(member.Id, fitnessClass.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(reservation, okResult.Value);
    }

    [Fact]
    public void CreateReservation_InvalidMember_ReturnsBadRequest()
    {
        // Act
        var result = _controller.CreateReservation(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid member or class.", badRequest.Value);
    }

    [Fact]
    public void CreateReservation_ServiceThrows_ReturnsBadRequest()
    {
        // Arrange
        var member = new Member { Id = Guid.NewGuid(), Name = "Test", MembershipType = MembershipType.Standard };
        var fitnessClass = new FitnessClass
        {
            Id = Guid.NewGuid(),
            Name = "Spin",
            Capacity = 1,
            StartTime = DateTime.Now
        };

        InMemoryDatabase.Members.Add(member);
        InMemoryDatabase.Classes.Add(fitnessClass);

        _reservationServiceMock
            .Setup(s => s.CreateReservation(member, fitnessClass))
            .Throws(new InvalidOperationException("Class capacity exceeded"));

        // Act
        var result = _controller.CreateReservation(member.Id, fitnessClass.Id);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Class capacity exceeded", badRequest.Value);
    }

    [Fact]
    public void GetAll_ReturnsOk()
    {
        // Act
        var result = _controller.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetById_NotFound_ReturnsNotFound()
    {
        // Act
        var result = _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Delete_ServiceThrows_ReturnsNotFound()
    {
        // Arrange
        _reservationServiceMock
            .Setup(s => s.CancelReservation(It.IsAny<Guid>()))
            .Throws(new InvalidOperationException("Reservation not found"));

        // Act
        var result = _controller.Delete(Guid.NewGuid());

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Reservation not found", notFound.Value);
    }
}
