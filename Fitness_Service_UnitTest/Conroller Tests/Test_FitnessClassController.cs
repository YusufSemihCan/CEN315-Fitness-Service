using Fitness_Service_API.Controllers;
using Fitness_Service_API.Entities;
using Fitness_Service_API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Fitness_Service_UnitTest.Controllers
{
    public class ClassesControllerTests
    {
        private readonly ClassesController _controller;

        public ClassesControllerTests()
        {
            _controller = new ClassesController();

            // CRITICAL: Reset static state to prevent test pollution
            InMemoryDatabase.Classes.Clear();
        }

        [Fact]
        public void CreateClass_ShouldAddToDatabase_AndReturnCreated()
        {
            // Arrange
            var fitnessClass = new FitnessClass
            {
                Name = "Pilates",
                Instructor = "Alice",
                Capacity = 15,
                StartTime = DateTime.Now.AddDays(1)
            };

            // Act
            var result = _controller.CreateClass(fitnessClass);

            // Assert
            // 1. Verify Return Type
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetClasses), createdResult.ActionName);

            // 2. Kill Mutant: "Void method call removal" (Did it actually save?)
            Assert.Single(InMemoryDatabase.Classes);
            Assert.Equal(fitnessClass.Id, InMemoryDatabase.Classes[0].Id);
        }

        [Fact]
        public void GetClasses_ShouldReturnAllClasses_FromDatabase()
        {
            // Arrange
            InMemoryDatabase.Classes.Add(new FitnessClass { Name = "Yoga" });
            InMemoryDatabase.Classes.Add(new FitnessClass { Name = "Spin" });

            // Act
            var result = _controller.GetClasses();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedClasses = Assert.IsAssignableFrom<List<FitnessClass>>(okResult.Value);

            Assert.Equal(2, returnedClasses.Count);
        }
    }
}