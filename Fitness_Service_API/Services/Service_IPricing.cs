using Fitness_Service_API.Entities;

namespace Fitness_Service_API.Services;

public interface IPricingService
{
    decimal CalculatePrice(
        MembershipType membershipType,
        DateTime classTime,
        int capacity,
        int currentReservations);
}
