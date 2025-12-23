using Fitness_Service_API.Entities;
using Fitness_Service_API.Services;
using Xunit;

namespace Fitness_Service_Test.Services
{
    public class PricingServiceTests
    {
        private readonly PricingService _service;

        // Helper date for "Off Peak" (e.g., 10:00 AM)
        private readonly DateTime _offPeakTime = new DateTime(2025, 1, 1, 10, 0, 0);

        public PricingServiceTests()
        {
            _service = new PricingService();
        }

        // =========================================================================
        // 1. BASE CONSTANT CHECK (Killing "Base Price" Mutants)
        // =========================================================================

        [Fact]
        public void CalculatePrice_ShouldReturnExactlyBasePrice_WhenNoMultipliersApply()
        {
            // Arrange
            var membership = MembershipType.Standard; // Multiplier: 1.0
            var time = _offPeakTime;                  // Multiplier: 1.0
            int capacity = 10;
            int reservations = 0;                     // Multiplier: 1.0

            // Act
            var price = _service.CalculatePrice(membership, time, capacity, reservations);

            // Assert
            // This strictly enforces that 'BasePrice' is exactly 100.
            // Any mutant that changes the constant 100m will fail this test.
            Assert.Equal(100m, price);
        }

        // =========================================================================
        // 2. BASE MEMBERSHIP LOGIC (Killing "Switch Case" Mutants)
        // =========================================================================

        [Theory]
        [InlineData(MembershipType.Standard, 100.0)] // 100 * 1.0
        [InlineData(MembershipType.Premium, 80.0)]   // 100 * 0.8
        [InlineData(MembershipType.Student, 70.0)]   // 100 * 0.7
        public void CalculatePrice_ShouldApplyMembershipMultiplier_WhenOffPeakAndEmpty(
            MembershipType type, decimal expectedPrice)
        {
            // Act
            decimal result = _service.CalculatePrice(type, _offPeakTime, 10, 0);

            // Assert
            Assert.Equal(expectedPrice, result);
        }

        [Fact]
        public void CalculatePrice_ShouldThrowException_ForInvalidMembership()
        {
            // This kills mutants that remove the "default" case in your switch statement
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _service.CalculatePrice((MembershipType)999, _offPeakTime, 10, 0));
        }

        // =========================================================================
        // 3. PEAK HOUR BOUNDARIES (Killing "Relational Operator" Mutants)
        // =========================================================================
        // Peak is 18:00 (inclusive) to 22:00 (exclusive) -> [18, 22)

        [Theory]
        [InlineData(17, 59, 100.0)] // Just before peak (Standard Price)
        [InlineData(18, 00, 120.0)] // Exact start of peak (100 * 1.2)
        [InlineData(21, 59, 120.0)] // Just before end of peak (100 * 1.2)
        [InlineData(22, 00, 100.0)] // Exact end of peak (Standard Price)
        public void CalculatePrice_ShouldApplyPeakMultiplier_OnlyWithinHours(
            int hour, int minute, decimal expectedPrice)
        {
            // Arrange
            var time = new DateTime(2025, 1, 1, hour, minute, 0);

            // Act
            decimal result = _service.CalculatePrice(MembershipType.Standard, time, 10, 0);

            // Assert
            Assert.Equal(expectedPrice, result);
        }

        // =========================================================================
        // 4. SURGE PRICING BOUNDARIES (Killing "Arithmetic" Mutants)
        // =========================================================================
        // Surge applies if occupancy > 80% (> 0.8)

        [Theory]
        [InlineData(10, 8, 100.0)]  // 8/10 = 0.8 (Not > 0.8) -> No Surge
        [InlineData(10, 9, 130.0)]  // 9/10 = 0.9 (> 0.8) -> Surge (100 * 1.3)
        [InlineData(100, 80, 100.0)] // 80/100 = 0.8 -> No Surge
        [InlineData(100, 81, 130.0)] // 81/100 = 0.81 -> Surge
        public void CalculatePrice_ShouldApplySurge_WhenOccupancyExceedsEightyPercent(
            int capacity, int reservations, decimal expectedPrice)
        {
            // Act
            decimal result = _service.CalculatePrice(
                MembershipType.Standard,
                _offPeakTime,
                capacity,
                reservations);

            // Assert
            Assert.Equal(expectedPrice, result);
        }

        [Fact]
        public void CalculatePrice_ShouldNotCrash_WhenCapacityIsZero()
        {
            // Kills mutants that might remove the "if (capacity > 0)" check
            // If that check is gone, this would throw DivideByZeroException
            decimal result = _service.CalculatePrice(MembershipType.Standard, _offPeakTime, 0, 0);

            Assert.Equal(100.0m, result); // Expect base price
        }

        // =========================================================================
        // 5. COMPLEX COMBINATIONS (Killing "Logic Hole" Mutants)
        // =========================================================================

        [Fact]
        public void CalculatePrice_ShouldCombineAllMultipliers_Correctly()
        {
            // Scenario: Student (0.7) + Peak (1.2) + Surge (1.3)
            // Base: 100
            // Calculation: 100 * 0.7 * 1.2 * 1.3 = 109.2

            var student = MembershipType.Student;
            var peakTime = new DateTime(2025, 1, 1, 19, 0, 0);
            int capacity = 10;
            int reservations = 9; // 90% -> Surge

            // Act
            decimal result = _service.CalculatePrice(student, peakTime, capacity, reservations);

            // Assert
            Assert.Equal(109.2m, result);
        }
    }
}