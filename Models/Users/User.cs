namespace Model.Users;

public class User
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required string FullName { get; init; }
    public required UserRole Role { get; init; }

    public override string ToString()
    {
        return $"{UserName}: {FullName}";
    }
}