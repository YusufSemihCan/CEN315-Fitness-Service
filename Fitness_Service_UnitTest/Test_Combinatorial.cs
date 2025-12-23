using Fitness_Service_API.Entities;
using Fitness_Service_API.Services;
using Xunit;

namespace Fitness_Service_UnitTest
{
    public class CombinatorialTests
    {
        private readonly PricingService _pricingService;

        public CombinatorialTests()
        {
            _pricingService = new PricingService();
        }

        // =========================================================================
        // REQUIREMENT 5.8: PAIRWISE TESTING (ACTS/PICT)
        // =========================================================================
        // These test cases were generated using Pairwise logic to ensure 
        // all pairs of (Membership, Time, Occupancy) are covered at least once.

        [Theory]
        //             MemberType              Hour   Res/Cap    Expected Calculation
        // Case 1: Standard + OffPeak + Low
        [InlineData(MembershipType.Standard, 10, 2, 10, 100.0)] // 100 * 1.0 * 1.0

        // Case 2: Standard + Peak + High (Surge)
        [InlineData(MembershipType.Standard, 19, 9, 10, 156.0)] // 100 * 1.2 * 1.3

        // Case 3: Premium + OffPeak + High (Surge)
        [InlineData(MembershipType.Premium, 10, 9, 10, 104.0)] // (100 * 0.8) * 1.3

        // Case 4: Premium + Peak + Low
        [InlineData(MembershipType.Premium, 19, 2, 10, 96.0)]  // (100 * 0.8) * 1.2

        // Case 5: Student + OffPeak + Low
        [InlineData(MembershipType.Student, 10, 2, 10, 70.0)]  // (100 * 0.7)

        // Case 6: Student + Peak + High (Surge)
        [InlineData(MembershipType.Student, 19, 9, 10, 109.2)] // (100 * 0.7) * 1.2 * 1.3
        public void CalculatePrice_Combinatorial_Scenarios(
            MembershipType member,
            int hour,
            int reservations,
            int capacity,
            decimal expectedPrice)
        {
            // Arrange
            var time = new DateTime(2025, 1, 1, hour, 0, 0);

            // Act
            decimal price = _pricingService.CalculatePrice(member, time, capacity, reservations);

            // Assert
            Assert.Equal(expectedPrice, price);
        }
    }
}