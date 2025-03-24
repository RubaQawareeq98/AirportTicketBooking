using Model.Users;

namespace Services.Users;

public class CurrentUser : ICurrentUser
{
    public required User User { get; set; }
}