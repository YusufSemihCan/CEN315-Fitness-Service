using System.Net;
using System.Net.Http.Json;
using Fitness_Service_API.Entities;
using Fitness_Service_API.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Fitness_Service_Tests
{
    // WebApplicationFactory<Program> spins up your real API in memory
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Create a client that talks to the in-memory API
            _client = factory.CreateClient();

            // CRITICAL: Clear the static DB so previous tests don't mess this up
            InMemoryDatabase.Members.Clear();
            InMemoryDatabase.Classes.Clear();
            InMemoryDatabase.Reservations.Clear();
        }

        [Fact]
        public async Task CreateMember_And_RetrieveIt_IntegrationFlow()
        {
            // 1. Arrange: Create a new member object
            var newMember = new Member
            {
                Name = "Integration Test User",
                MembershipType = MembershipType.Premium
            };

            // 2. Act: Send a POST request to /members
            var postResponse = await _client.PostAsJsonAsync("/members", newMember);

            // 3. Assert: Verify creation was successful (HTTP 201 Created)
            postResponse.EnsureSuccessStatusCode(); // Throws if not 200-299
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            // 4. Act: Extract the ID from the response to verify we can GET it
            var createdMember = await postResponse.Content.ReadFromJsonAsync<Member>();
            Assert.NotNull(createdMember);

            // 5. Act: Send a GET request to /members/{id}
            var getResponse = await _client.GetAsync($"/members/{createdMember.Id}");

            // 6. Assert: Verify we can fetch the data we just saved
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var fetchedMember = await getResponse.Content.ReadFromJsonAsync<Member>();

            Assert.Equal(newMember.Name, fetchedMember.Name);
            Assert.Equal(MembershipType.Premium, fetchedMember.MembershipType);
        }

        [Fact]
        public async Task Full_Flow_Create_Class_And_Book_Reservation()
        {
            // --- Step A: Create Member ---
            var member = new Member { Name = "Booking User", MembershipType = MembershipType.Standard };
            var memResponse = await _client.PostAsJsonAsync("/members", member);
            var createdMember = await memResponse.Content.ReadFromJsonAsync<Member>();

            // --- Step B: Create Class ---
            // FIX 1: Set a specific "Safe" time (Noon tomorrow) so we never hit Peak Hours (18-22)
            var safeTime = DateTime.Today.AddDays(1).AddHours(12);

            var fitnessClass = new FitnessClass
            {
                Name = "Zumba Integration",
                Capacity = 5,
                StartTime = safeTime
            };
            var classResponse = await _client.PostAsJsonAsync("/classes", fitnessClass);
            var createdClass = await classResponse.Content.ReadFromJsonAsync<FitnessClass>();

            // --- Step C: Create Reservation ---
            var resResponse = await _client.PostAsync(
                $"/reservations?memberId={createdMember.Id}&classId={createdClass.Id}",
                null);

            // --- Step D: Assert Success ---
            resResponse.EnsureSuccessStatusCode();
            var reservation = await resResponse.Content.ReadFromJsonAsync<Reservation>();

            Assert.NotNull(reservation);
            Assert.Equal(createdMember.Id, reservation.MemberId);

            // FIX 2: Expect 100 (Base Price) instead of 20
            Assert.Equal(100m, reservation.PricePaid);
        }
    }
}