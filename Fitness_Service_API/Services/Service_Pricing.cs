using Fitness_Service_API.Entities;

namespace Fitness_Service_API.Services;

public class PricingService : IPricingService
{
    private readonly decimal BasePrice = 100m;

    public decimal CalculatePrice(
        MembershipType membershipType,
        DateTime classTime,
        int capacity,
        int currentReservations)
    {
        decimal price = BasePrice;

        // Membership-based pricing
        price *= membershipType switch
        {
            MembershipType.Standard => 1.0m,
            MembershipType.Premium => 0.8m,
            MembershipType.Student => 0.7m,
            _ => throw new ArgumentOutOfRangeException(nameof(membershipType))
        };

        // Peak hours: 18:00 - 22:00
        if (classTime.Hour >= 18 && classTime.Hour < 22)
        {
            price *= 1.2m;
        }

        // Surge pricing if occupancy > 80%
        if (capacity > 0)
        {
            decimal occupancyRate = (decimal)currentReservations / capacity;
            if (occupancyRate > 0.8m)
            {
                price *= 1.3m;
            }
        }

        return Math.Round(price, 2);
    }
}
