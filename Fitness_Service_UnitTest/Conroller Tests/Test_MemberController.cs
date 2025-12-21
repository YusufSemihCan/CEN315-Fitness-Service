using Fitness_Service_API.Controllers;
using Fitness_Service_API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Service_UnitTest;

public class MembersControllerTests
{
    [Fact]
    public void AddMember_ReturnsCreatedResult()
    {
        // Arrange
        var controller = new MembersController();
        var member = new Member
        {
            Name = "Test User",
            MembershipType = MembershipType.Standard
        };

        // Act
        var result = controller.AddMember(member);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedMember = Assert.IsType<Member>(createdResult.Value);
        Assert.Equal("Test User", returnedMember.Name);
    }

    [Fact]
    public void GetMember_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var controller = new MembersController();
        var invalidId = Guid.NewGuid();

        // Act
        var result = controller.GetMember(invalidId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
