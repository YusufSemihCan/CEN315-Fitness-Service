using Fitness_Service_API.Entities;

namespace Fitness_Service_API.Services;

public interface IReservationService
{
    Reservation CreateReservation(Member member, FitnessClass fitnessClass);
    void CancelReservation(Guid reservationId);
}
