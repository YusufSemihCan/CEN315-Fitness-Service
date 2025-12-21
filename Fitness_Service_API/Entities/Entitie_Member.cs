namespace Fitness_Service_API.Entities;


public enum MembershipType
{
    Standard,
    Premium,
    Student
}

public class Member
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public MembershipType MembershipType { get; set; }
}
