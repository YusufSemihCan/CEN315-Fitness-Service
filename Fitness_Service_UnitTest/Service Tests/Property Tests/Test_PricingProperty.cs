using Fitness_Service_API.Entities;
using Fitness_Service_API.Services;
using FsCheck;
using FsCheck.Xunit;
using System;

namespace Fitness_Service_Test
{
    public class PricingPropertiesTests
    {
        private readonly PricingService _pricingService = new PricingService();

        // 1. MONOTONICITY: Premium is always <= Standard
        // This kills mutants that mess up the membership multipliers
        [Property]
        public void Price_For_Premium_Is_Always_Less_Or_Equal_To_Standard(
            DateTime time,
            PositiveInt capacity,
            NonNegativeInt reservations)
        {
            // Arrange
            int validReservations = Math.Min(reservations.Get, capacity.Get);

            // Act
            decimal stdPrice = _pricingService.CalculatePrice(MembershipType.Standard, time, capacity.Get, validReservations);
            decimal premPrice = _pricingService.CalculatePrice(MembershipType.Premium, time, capacity.Get, validReservations);

            // Assert
            Assert.True(premPrice <= stdPrice,
                $"Premium ({premPrice}) should be <= Standard ({stdPrice}) for same conditions");
        }

        // 2. SURGE LOGIC: Higher occupancy >= Lower occupancy
        // This kills mutants in the "occupancy > 0.8" check
        [Property]
        public void Price_Increases_Or_Stays_Same_With_Occupancy(
            MembershipType type,
            DateTime time,
            PositiveInt capacity)
        {
            // Arrange
            int lowRes = (int)(capacity.Get * 0.1); // 10% full
            int highRes = (int)(capacity.Get * 0.9); // 90% full (Triggers surge)

            // Act
            decimal lowPrice = _pricingService.CalculatePrice(type, time, capacity.Get, lowRes);
            decimal highPrice = _pricingService.CalculatePrice(type, time, capacity.Get, highRes);

            // Assert
            Assert.True(highPrice >= lowPrice,
                $"Surge price ({highPrice}) should be >= Regular price ({lowPrice})");
        }

        // 3. PEAK LOGIC: Peak hour >= Off Peak
        // This kills mutants in the "Hour >= 18 && Hour < 22" check
        [Property]
        public void Peak_Hour_Price_Is_Always_Higher_Or_Equal(
            MembershipType type,
            PositiveInt capacity,
            NonNegativeInt reservations)
        {
            // Arrange
            int validRes = Math.Min(reservations.Get, capacity.Get);

            var offPeakTime = new DateTime(2025, 1, 1, 10, 0, 0); // 10:00
            var peakTime = new DateTime(2025, 1, 1, 19, 0, 0);    // 19:00

            // Act
            decimal offPeakPrice = _pricingService.CalculatePrice(type, offPeakTime, capacity.Get, validRes);
            decimal peakPrice = _pricingService.CalculatePrice(type, peakTime, capacity.Get, validRes);

            // Assert
            Assert.True(peakPrice >= offPeakPrice,
                $"Peak price ({peakPrice}) should be >= Off-Peak ({offPeakPrice})");
        }
    }
}