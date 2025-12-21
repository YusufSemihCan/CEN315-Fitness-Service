namespace Fitness_Service_API.Entities;

public class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MemberId { get; set; }
    public Guid FitnessClassId { get; set; }
    public decimal PricePaid { get; set; }
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
}