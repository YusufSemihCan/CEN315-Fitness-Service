using Fitness_Service_API.Entities;
using Fitness_Service_API.Services;
using FsCheck;
using FsCheck.Xunit;

namespace Fitness_Service_UnitTest
{
    public class ReservationPropertiesTests
    {
        [Property]
        public void Reservation_Count_Never_Exceeds_Capacity(
            PositiveInt capacity,
            NonNegativeInt attempts)
        {
            // Arrange
            var pricingService = new PricingService();
            var reservationService = new ReservationService(pricingService);

            var member = new Member
            {
                Id = Guid.NewGuid(),
                MembershipType = MembershipType.Standard
            };

            var fitnessClass = new FitnessClass
            {
                Id = Guid.NewGuid(),
                Capacity = capacity.Get,
                StartTime = DateTime.Now
            };

            // Act
            for (int i = 0; i < attempts.Get; i++)
            {
                try
                {
                    reservationService.CreateReservation(member, fitnessClass);
                }
                catch
                {
                    // Expected when capacity is exceeded
                }
            }

            // Assert (INVARIANT)
            Assert.True(fitnessClass.Reservations.Count <= fitnessClass.Capacity);
        }
    }
}
