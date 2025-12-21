using Fitness_Service_API.Services;
using Fitness_Service_API.Infrastructure;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public IActionResult CreateReservation(Guid memberId, Guid classId)
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

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(InMemoryDatabase.Reservations);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var reservation = InMemoryDatabase.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
            return NotFound();

        return Ok(reservation);
    }

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
