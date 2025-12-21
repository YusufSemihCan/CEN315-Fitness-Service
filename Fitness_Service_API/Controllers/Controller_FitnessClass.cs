using Fitness_Service_API.Entities;
using Fitness_Service_API.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Service_API.Controllers;

[ApiController]
[Route("classes")]
public class ClassesController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateClass(FitnessClass fitnessClass)
    {
        InMemoryDatabase.Classes.Add(fitnessClass);
        return CreatedAtAction(nameof(GetClasses), new { id = fitnessClass.Id }, fitnessClass);
    }

    [HttpGet]
    public IActionResult GetClasses()
    {
        return Ok(InMemoryDatabase.Classes);
    }
}
