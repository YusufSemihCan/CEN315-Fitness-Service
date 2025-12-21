using Fitness_Service_API.Entities;
using Fitness_Service_API.Services;
using FsCheck;
using FsCheck.Xunit;

namespace Fitness_Service_UnitTest
{
    public class PricingPropertiesTests
    {
        [Property]
        public void Calculated_Price_Is_Always_Positive_And_Bounded(
            MembershipType membershipType,
            PositiveInt capacity,
            NonNegativeInt reservations,
            DateTime classTime)
        {
            // Arrange
            var pricingService = new PricingService();

            int currentReservations = Math.Min(reservations.Get, capacity.Get);

            // Act
            var price = pricingService.CalculatePrice(
                membershipType,
                classTime,
                capacity.Get,
                currentReservations);

            // Assert (PROPERTIES)
            Assert.True(price > 0);
            Assert.True(price <= 500); // upper bound safety rule
        }
    }
}
