using Fitness_Service_API.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace Fitness_Service_UnitTest;

public class ClassesControllerTests
{
    [Fact]
    public void CreateClass_ReturnsCreatedClass()
    {
        // Arrange
        var controller = new ClassesController();
        var fitnessClass = new FitnessClass
        {
            Name = "Yoga",
            Instructor = "Alice",
            Capacity = 10,
            StartTime = DateTime.Today.AddHours(10)
        };

        // Act
        var result = controller.CreateClass(fitnessClass);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedClass = Assert.IsType<FitnessClass>(createdResult.Value);
        Assert.Equal("Yoga", returnedClass.Name);
    }

    [Fact]
    public void GetClasses_ReturnsOkResult()
    {
        // Arrange
        var controller = new ClassesController();

        // Act
        var result = controller.GetClasses();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
