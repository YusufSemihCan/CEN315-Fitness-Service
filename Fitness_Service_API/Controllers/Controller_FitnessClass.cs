using Fitness_Service_API.Entities;
using FitnessService.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FitnessService.Api.Controllers;

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

    [HttpDelete("{id}")]
    public IActionResult DeleteClass(Guid id)
    {
        var fitnessClass = InMemoryDatabase.Classes.FirstOrDefault(c => c.Id == id);
        if (fitnessClass == null)
            return NotFound();

        InMemoryDatabase.Classes.Remove(fitnessClass);
        return NoContent();
    }

}
