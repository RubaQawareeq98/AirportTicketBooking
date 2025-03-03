namespace Model;

public class User
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public required UserRole Role { get; set; }

    public override string ToString()
    {
        return $"{Id} {Email} {FullName} {Password} {Role}";
    }
}