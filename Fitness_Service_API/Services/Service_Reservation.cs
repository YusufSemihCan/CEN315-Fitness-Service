using Fitness_Service_API.Entities;
using FitnessService.Infrastructure;

namespace FitnessService.Domain.Services;

public class ReservationService : IReservationService
{
    private readonly IPricingService _pricingService;

    public ReservationService(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public Reservation CreateReservation(Member member, FitnessClass fitnessClass)
    {
        if (fitnessClass.Reservations.Count >= fitnessClass.Capacity)
            throw new InvalidOperationException("Class capacity exceeded.");

        decimal price = _pricingService.CalculatePrice(
            member.MembershipType,
            fitnessClass.StartTime,
            fitnessClass.Capacity,
            fitnessClass.Reservations.Count);

        var reservation = new Reservation
        {
            MemberId = member.Id,
            FitnessClassId = fitnessClass.Id,
            PricePaid = price
        };

        fitnessClass.Reservations.Add(reservation);
        InMemoryDatabase.Reservations.Add(reservation);

        return reservation;
    }

    public void CancelReservation(Guid reservationId)
    {
        var reservation = InMemoryDatabase.Reservations
            .FirstOrDefault(r => r.Id == reservationId);

        if (reservation == null)
            throw new InvalidOperationException("Reservation not found.");

        InMemoryDatabase.Reservations.Remove(reservation);
    }
}
