using Model.Users;

namespace Services.Users;

public class CurrentUser : ICurrentUser
{
    public User User { get; set; }
}