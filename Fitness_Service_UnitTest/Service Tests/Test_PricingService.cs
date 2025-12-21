using Fitness_Service_API.Entities;
using Fitness_Service_API.Services;

namespace Fitness_Service_UnitTest;

public class PricingServiceTests
{
    private readonly PricingService _pricingService;

    public PricingServiceTests()
    {
        _pricingService = new PricingService();
    }

    [Fact]
    public void CalculatePrice_ShouldIncreasePrice_DuringPeakHours()
    {
        // Arrange
        var membershipType = MembershipType.Standard;
        var peakHourTime = new DateTime(2025, 1, 1, 19, 0, 0); // 19:00
        int capacity = 20;
        int currentReservations = 5;

        // Base price = 100
        // Peak hour multiplier = 1.2
        decimal expectedPrice = 120m;

        // Act
        var price = _pricingService.CalculatePrice(
            membershipType,
            peakHourTime,
            capacity,
            currentReservations);

        // Assert
        Assert.Equal(expectedPrice, price);
    }
}
