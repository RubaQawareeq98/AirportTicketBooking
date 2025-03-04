using Model.Users;

namespace Services.Users;

public interface ICurrentUser
{
    User User { get; set; }
}