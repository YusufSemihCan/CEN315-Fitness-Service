using Fitness_Service_API.Controllers;
using Fitness_Service_API.Entities;
using Fitness_Service_API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Fitness_Service_Test.Controllers
{
    public class MembersControllerTests
    {
        private readonly MembersController _controller;

        public MembersControllerTests()
        {
            _controller = new MembersController();

            // CRITICAL: Reset static state
            InMemoryDatabase.Members.Clear();
        }

        [Fact]
        public void AddMember_ShouldAddToDatabase_AndReturnCreated()
        {
            // Arrange
            var member = new Member
            {
                Name = "John Doe",
                MembershipType = MembershipType.Premium
            };

            // Act
            var result = _controller.AddMember(member);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            // Verify Logic: Route values must contain the new ID
            // This kills mutants that might pass a null or wrong ID to the result
            Assert.NotNull(createdResult.RouteValues);
            Assert.Equal(member.Id, createdResult.RouteValues["id"]);

            // Verify State: Item must exist in the static list
            Assert.Single(InMemoryDatabase.Members);
            Assert.Equal("John Doe", InMemoryDatabase.Members[0].Name);
        }

        [Fact]
        public void GetMember_ShouldReturnOk_WhenFound()
        {
            // Arrange
            var member = new Member { Id = Guid.NewGuid(), Name = "Jane" };
            InMemoryDatabase.Members.Add(member);

            // Act
            var result = _controller.GetMember(member.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMember = Assert.IsType<Member>(okResult.Value);
            Assert.Equal(member.Id, returnedMember.Id);
        }

        [Fact]
        public void GetMember_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            InMemoryDatabase.Members.Clear(); // Ensure empty
            var randomId = Guid.NewGuid();

            // Act
            var result = _controller.GetMember(randomId);

            // Assert
            // This kills mutants that might change '==' to '!=' in the lookup
            Assert.IsType<NotFoundResult>(result);
        }
    }
}