using Fitness_Service_API.Entities;

namespace Fitness_Service_API.Infrastructure;

public static class InMemoryDatabase
{
    public static List<Member> Members { get; } = new();
    public static List<FitnessClass> Classes { get; } = new();
    public static List<Reservation> Reservations { get; } = new();
}
