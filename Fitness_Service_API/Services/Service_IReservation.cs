using Fitness_Service_API.Entities;

namespace FitnessService.Domain.Services;

public interface IReservationService
{
    Reservation CreateReservation(Member member, FitnessClass fitnessClass);
    void CancelReservation(Guid reservationId);
}
