using Fitness_Service_API.Entities;
using Fitness_Service_API.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_Service_API.Controllers;

[ApiController]
[Route("members")]
public class MembersController : ControllerBase
{
    [HttpPost]
    public IActionResult AddMember(Member member)
    {
        InMemoryDatabase.Members.Add(member);
        return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
    }

    [HttpGet("{id}")]
    public IActionResult GetMember(Guid id)
    {
        var member = InMemoryDatabase.Members.FirstOrDefault(m => m.Id == id);
        if (member == null)
            return NotFound();

        return Ok(member);
    }
}
