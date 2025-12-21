using Fitness_Service_API.Entities;

public class FitnessClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public DateTime StartTime { get; set; }

    public List<Reservation> Reservations { get; set; } = new();
}
