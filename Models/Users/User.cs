namespace Model.Users;

public class User
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public required UserRole Role { get; set; }

    public override string ToString()
    {
        return $"{UserName}: {FullName}";
    }
}