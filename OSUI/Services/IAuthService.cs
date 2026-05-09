using OSUI.Models;

namespace OSUI.Services;

public interface IAuthService
{
    UserModel? CurrentUser { get; }
    bool IsLoggedIn { get; }
    bool Login(string username, string password);
    bool Register(string username, string password);
    void NotifyRoleChanged(UserModel updatedUser);
}

