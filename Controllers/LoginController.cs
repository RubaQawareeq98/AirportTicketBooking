using Model.Users;
using Services.Users;
using Views;

namespace Controllers;

public class LoginController (ILoginView loginView, IUserService userService)
{
    public async Task<User> Login()
    {
        loginView.WelcomeMessage();
        var userName = loginView.ReadUserName();
        var password = loginView.ReadPassword();
        var user = await userService.ValidateUser(userName, password);
        return user;
    }
}