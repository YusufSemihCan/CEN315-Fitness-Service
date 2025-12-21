using Fitness_Service_API.Entities;
using Fitness_Service_API.Services;
using FsCheck;
using FsCheck.Xunit;
using Moq;
using System;
using System.Collections.Generic;

namespace Fitness_Service_UnitTest
{
    public class ReservationPropertiesTests
    {
        [Property]
        public void Reservation_Final_Count_Matches_Min_Of_Attempts_And_Capacity(
            PositiveInt capacity,
            PositiveInt attempts)
        {
            // Arrange
            // We mock pricing because we don't care about price here, only the List Logic
            var pricingMock = new Mock<IPricingService>();
            var service = new ReservationService(pricingMock.Object);

            var fitnessClass = new FitnessClass
            {
                Id = Guid.NewGuid(),
                Capacity = capacity.Get,
                StartTime = DateTime.Now,
                Reservations = new List<Reservation>()
            };

            var member = new Member { Id = Guid.NewGuid() };

            // Act
            // Try to add 'attempts.Get' number of reservations
            for (int i = 0; i < attempts.Get; i++)
            {
                try
                {
                    // We need unique members if your logic checks for duplicates, 
                    // but based on your code, it only checks capacity.
                    // To be safe, we create a new member object every time.
                    service.CreateReservation(new Member { Id = Guid.NewGuid() }, fitnessClass);
                }
                catch
                {
                    // Ignore expected capacity exceptions
                }
            }

            // Assert (The Strong Invariant)
            // If attempts < capacity, everyone should get in.
            // If attempts > capacity, the class should be exactly full.
            int expectedCount = Math.Min(attempts.Get, capacity.Get);

            Assert.Equal(expectedCount, fitnessClass.Reservations.Count);
        }
    }
}