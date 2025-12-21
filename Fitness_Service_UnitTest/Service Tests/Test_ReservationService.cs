using Fitness_Service_API.Entities;
using Fitness_Service_API.Services;
using Fitness_Service_API.Infrastructure;
using Moq;
using Xunit;

// CRITICAL: Forces tests to run one-by-one to prevent "Test Pollution" in the static InMemoryDatabase
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Fitness_Service_UnitTest.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IPricingService> _pricingServiceMock;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _pricingServiceMock = new Mock<IPricingService>();

            // We test the REAL service, injecting the mocked dependency
            _service = new ReservationService(_pricingServiceMock.Object);

            // CRITICAL: Clean the static DB before EVERY test
            // This ensures a clean slate so "count" based tests don't fail randomly
            InMemoryDatabase.Members.Clear();
            InMemoryDatabase.Classes.Clear();
            InMemoryDatabase.Reservations.Clear();
        }

        // =========================================================================
        // 1. SUCCESS SCENARIOS (Killing "No-Op", "Data Flow", and "DB Add" Mutants)
        // =========================================================================

        [Fact]
        public void CreateReservation_ShouldAddReservationToClass_WhenSpaceAvailable()
        {
            // Arrange
            var member = new Member { Id = Guid.NewGuid(), MembershipType = MembershipType.Premium };
            var fitnessClass = new FitnessClass
            {
                Id = Guid.NewGuid(),
                Capacity = 10,
                StartTime = DateTime.Now.AddHours(1),
                Reservations = new List<Reservation>() // Start empty
            };

            decimal expectedPrice = 50.0m;

            // Strict Mocking: Ensure the service asks for price using the CORRECT data
            _pricingServiceMock
                .Setup(p => p.CalculatePrice(
                    member.MembershipType,
                    fitnessClass.StartTime,
                    fitnessClass.Capacity,
                    0)) // Current reservations is 0
                .Returns(expectedPrice)
                .Verifiable();

            // Act
            var result = _service.CreateReservation(member, fitnessClass);

            // Assert
            // 1. Verify Object Return
            Assert.NotNull(result);
            Assert.Equal(expectedPrice, result.PricePaid);
            Assert.Equal(member.Id, result.MemberId);

            // 2. Verify Domain Object Update (Kills "fitnessClass.Add" removal mutant)
            Assert.Single(fitnessClass.Reservations);
            Assert.Contains(result, fitnessClass.Reservations);

            // 3. Verify Database Update (Kills "InMemoryDatabase.Add" removal mutant)
            Assert.Single(InMemoryDatabase.Reservations);
            Assert.Contains(result, InMemoryDatabase.Reservations);

            // 4. Verify Interaction
            _pricingServiceMock.Verify();
        }

        // =========================================================================
        // 2. BOUNDARY & LOGIC SCENARIOS (Killing "Conditional Boundary" Mutants)
        // =========================================================================

        [Fact]
        public void CreateReservation_ShouldThrowException_WhenClassIsExactlyFull()
        {
            // Arrange
            var member = new Member { Id = Guid.NewGuid() };
            var fitnessClass = new FitnessClass { Capacity = 2, Reservations = new List<Reservation>() };

            // Fill the class to capacity
            fitnessClass.Reservations.Add(new Reservation());
            fitnessClass.Reservations.Add(new Reservation());

            // Act & Assert
            // This kills mutants that change '<' to '<=' in capacity checks
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _service.CreateReservation(member, fitnessClass));

            Assert.Equal("Class capacity exceeded.", ex.Message);
        }

        [Fact]
        public void CreateReservation_ShouldPassCorrectReservationCount_ToPricing()
        {
            // Arrange
            var member = new Member { Id = Guid.NewGuid() };
            var fitnessClass = new FitnessClass { Capacity = 10, StartTime = DateTime.Now };

            // Pre-fill with 3 people
            fitnessClass.Reservations.Add(new Reservation());
            fitnessClass.Reservations.Add(new Reservation());
            fitnessClass.Reservations.Add(new Reservation());

            // Act
            _service.CreateReservation(member, fitnessClass);

            // Assert
            // Kills mutants that hardcode "0" or "1" instead of counting the list
            _pricingServiceMock.Verify(p => p.CalculatePrice(
                It.IsAny<MembershipType>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                3), // Must match exactly 3
                Times.Once);
        }

        // =========================================================================
        // 3. DEFENSIVE CODING (Killing "Null Check Removal" Mutants)
        // =========================================================================

        [Fact]
        public void CreateReservation_ShouldThrowArgumentNull_WhenInputsAreNull()
        {
            // Kills mutant: Removing "if (member == null)"
            Assert.Throws<ArgumentNullException>(() => _service.CreateReservation(null, new FitnessClass()));

            // Kills mutant: Removing "if (fitnessClass == null)"
            Assert.Throws<ArgumentNullException>(() => _service.CreateReservation(new Member(), null));
        }

        // =========================================================================
        // 4. CANCELLATION LOGIC (Killing "DB Remove" Mutants)
        // =========================================================================

        [Fact]
        public void CancelReservation_ShouldRemoveFromDatabase_WhenFound()
        {
            // Arrange
            var targetId = Guid.NewGuid();
            var reservation = new Reservation { Id = targetId };

            // Manually add to DB to simulate it existing
            InMemoryDatabase.Reservations.Add(reservation);

            // Act
            _service.CancelReservation(targetId);

            // Assert
            // Kills mutant: Removing "InMemoryDatabase.Reservations.Remove(...)"
            Assert.Empty(InMemoryDatabase.Reservations);
            Assert.DoesNotContain(reservation, InMemoryDatabase.Reservations);
        }

        [Fact]
        public void CancelReservation_ShouldThrow_WhenNotFound()
        {
            // Arrange
            InMemoryDatabase.Reservations.Clear(); // Ensure empty

            // Act & Assert
            // Kills mutant: Removing "if (reservation == null)"
            var ex = Assert.Throws<InvalidOperationException>(() =>
                _service.CancelReservation(Guid.NewGuid()));

            Assert.Equal("Reservation not found.", ex.Message);
        }
    }
}