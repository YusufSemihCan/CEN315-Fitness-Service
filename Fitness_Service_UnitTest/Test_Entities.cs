using Fitness_Service_API.Entities;
using Xunit;

namespace Fitness_Service_Test.Entities
{
    public class EntityTests
    {
        // =========================================================================
        // 1. FITNESS CLASS TESTS (The Toughest Mutants)
        // =========================================================================

        [Fact]
        public void FitnessClass_Constructor_ShouldInitializeDefaults_Correctly()
        {
            // Act
            var fitnessClass = new FitnessClass();

            // Assert
            // Kills mutant: Removing "= new()" (Reservations would be null)
            Assert.NotNull(fitnessClass.Reservations);
            Assert.Empty(fitnessClass.Reservations);

            // Kills mutant: Removing "= string.Empty" (Name would be null)
            Assert.NotNull(fitnessClass.Name);
            Assert.Equal(string.Empty, fitnessClass.Name);

            // Kills mutant: Removing "= string.Empty" (Instructor would be null)
            Assert.NotNull(fitnessClass.Instructor);
            Assert.Equal(string.Empty, fitnessClass.Instructor);

            // Kills mutant: Removing "= Guid.NewGuid()"
            Assert.NotEqual(Guid.Empty, fitnessClass.Id);
        }

        [Fact]
        public void FitnessClass_Setters_ShouldUpdateValues()
        {
            // Arrange
            var fitnessClass = new FitnessClass();
            var newId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var newList = new List<Reservation> { new Reservation() };

            // Act
            fitnessClass.Id = newId;
            fitnessClass.Name = "Zumba";
            fitnessClass.Instructor = "Leo";
            fitnessClass.Capacity = 25;
            fitnessClass.StartTime = now;
            fitnessClass.Reservations = newList;

            // Assert (Verifies properties are not ReadOnly or "stuck")
            Assert.Equal(newId, fitnessClass.Id);
            Assert.Equal("Zumba", fitnessClass.Name);
            Assert.Equal("Leo", fitnessClass.Instructor);
            Assert.Equal(25, fitnessClass.Capacity);
            Assert.Equal(now, fitnessClass.StartTime);
            Assert.Same(newList, fitnessClass.Reservations);
        }

        [Fact]
        public void FitnessClass_Ids_ShouldBeUnique()
        {
            // Kills mutant: Replacing Guid.NewGuid() with a static/constant GUID
            var c1 = new FitnessClass();
            var c2 = new FitnessClass();
            Assert.NotEqual(c1.Id, c2.Id);
        }

        // =========================================================================
        // 2. MEMBER TESTS
        // =========================================================================

        [Fact]
        public void Member_Constructor_ShouldInitializeDefaults()
        {
            // Act
            var member = new Member();

            // Assert
            Assert.NotEqual(Guid.Empty, member.Id);
            Assert.NotNull(member.Name); // Kills null string mutant
            Assert.Equal(string.Empty, member.Name);
            Assert.Equal(MembershipType.Standard, member.MembershipType); // Default enum value
        }

        [Fact]
        public void Member_Setters_ShouldUpdateValues()
        {
            // Arrange
            var member = new Member();
            var newId = Guid.NewGuid();

            // Act
            member.Id = newId;
            member.Name = "Alice";
            member.MembershipType = MembershipType.Premium;

            // Assert
            Assert.Equal(newId, member.Id);
            Assert.Equal("Alice", member.Name);
            Assert.Equal(MembershipType.Premium, member.MembershipType);
        }

        [Fact]
        public void Member_Ids_ShouldBeUnique()
        {
            var m1 = new Member();
            var m2 = new Member();
            Assert.NotEqual(m1.Id, m2.Id);
        }

        // =========================================================================
        // 3. RESERVATION TESTS
        // =========================================================================

        [Fact]
        public void Reservation_Constructor_ShouldInitializeDefaults()
        {
            // Act
            var reservation = new Reservation();

            // Assert
            Assert.NotEqual(Guid.Empty, reservation.Id);

            // Kills mutant: Removing "= DateTime.UtcNow" or changing it to MinValue
            // We check that the date is "recent" (created within the last 1 second)
            var now = DateTime.UtcNow;
            Assert.True((now - reservation.ReservedAt).TotalSeconds < 1, "ReservedAt should be set to Now");
        }

        [Fact]
        public void Reservation_Setters_ShouldUpdateValues()
        {
            // Arrange
            var reservation = new Reservation();
            var newId = Guid.NewGuid();
            var mId = Guid.NewGuid();
            var cId = Guid.NewGuid();
            var date = DateTime.UtcNow.AddDays(1);

            // Act
            reservation.Id = newId;
            reservation.MemberId = mId;
            reservation.FitnessClassId = cId;
            reservation.PricePaid = 99.99m;
            reservation.ReservedAt = date;

            // Assert
            Assert.Equal(newId, reservation.Id);
            Assert.Equal(mId, reservation.MemberId);
            Assert.Equal(cId, reservation.FitnessClassId);
            Assert.Equal(99.99m, reservation.PricePaid);
            Assert.Equal(date, reservation.ReservedAt);
        }

        [Fact]
        public void Reservation_Ids_ShouldBeUnique()
        {
            var r1 = new Reservation();
            var r2 = new Reservation();
            Assert.NotEqual(r1.Id, r2.Id);
        }
    }
}