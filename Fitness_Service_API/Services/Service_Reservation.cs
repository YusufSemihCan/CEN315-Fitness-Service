using Fitness_Service_API.Entities;
using Fitness_Service_API.Infrastructure;

namespace Fitness_Service_API.Services;

public class ReservationService : IReservationService
{
    private readonly IPricingService _pricingService;

    public ReservationService(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public Reservation CreateReservation(Member member, FitnessClass fitnessClass)
    {

        if (member == null)
            throw new ArgumentNullException(nameof(member));

        if (fitnessClass == null)
            throw new ArgumentNullException(nameof(fitnessClass));


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
