using Fitness_Service_API.Entities;

namespace FitnessService.Domain.Services;

public interface IPricingService
{
    decimal CalculatePrice(
        MembershipType membershipType,
        DateTime classTime,
        int capacity,
        int currentReservations);
}
