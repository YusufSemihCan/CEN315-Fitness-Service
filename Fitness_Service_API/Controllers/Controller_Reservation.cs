using FitnessService.Domain.Services;
using FitnessService.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FitnessService.Api.Controllers;

[ApiController]
[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    // POST /reservations
    [HttpPost]
    public IActionResult Create(Guid memberId, Guid classId)
    {
        var member = InMemoryDatabase.Members.FirstOrDefault(m => m.Id == memberId);
        var fitnessClass = InMemoryDatabase.Classes.FirstOrDefault(c => c.Id == classId);

        if (member == null || fitnessClass == null)
            return BadRequest("Invalid member or class.");

        try
        {
            var reservation = _reservationService.CreateReservation(member, fitnessClass);
            return Ok(reservation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET /reservations
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(InMemoryDatabase.Reservations);
    }

    // GET /reservations/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var reservation = InMemoryDatabase.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
            return NotFound();

        return Ok(reservation);
    }

    // DELETE /reservations/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            _reservationService.CancelReservation(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
