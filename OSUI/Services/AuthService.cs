using CommunityToolkit.Mvvm.Messaging;
using OSUI.Models;

namespace OSUI.Services;

public sealed class AuthService : IAuthService
{
    #region 状态

    /// <summary>当前登录的用户；未登录时为 null。</summary>
    public UserModel? CurrentUser { get; private set; }


    /// <summary>是否已登录。</summary>
    public bool IsLoggedIn => CurrentUser is not null;

    #endregion

    #region 操作

    /// <summary>
    /// 尝试用用户名和明文密码登录
    /// </summary>
    /// <returns>登录成功返回 true，否则 false。</returns>
    public bool Login(string username, string password)
    {
        var users = DataService.LoadUsers();
        var hash = PasswordService.ComputeHash(password);
        var user = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
            && u.PasswordHash == hash
        );

        if (user is null)
            return false;

        CurrentUser = user;
        return true;
    }

    /// <summary>
    /// 注册新用户
    /// </summary>
    public bool Register(string username, string password)
    {
        var users = DataService.LoadUsers();
        if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
        {
            return false; // 用户已存在
        }

        var newUser = new UserModel
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = PasswordService.ComputeHash(password)
        };

        users.Add(newUser);
        DataService.SaveUsers(users);
        return true;
    }

    /// <summary>
    /// 修改当前登录用户的密码
    /// </summary>
    public bool ChangePassword(string currentPassword, string newPassword, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (CurrentUser is null)
        {
            errorMessage = LocalizationService.Instance.GetString("Auth.Error.LoginRequired");
            return false;
        }

        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            errorMessage = LocalizationService.Instance.GetString("Auth.Error.PasswordEmpty");
            return false;
        }

        var currentHash = PasswordService.ComputeHash(currentPassword);
        if (!string.Equals(CurrentUser.PasswordHash, currentHash, StringComparison.Ordinal))
        {
            errorMessage = LocalizationService.Instance.GetString("Auth.Error.CurrentPasswordIncorrect");
            return false;
        }

        var newHash = PasswordService.ComputeHash(newPassword);
        if (string.Equals(CurrentUser.PasswordHash, newHash, StringComparison.Ordinal))
        {
            errorMessage = LocalizationService.Instance.GetString("Auth.Error.NewPasswordSameAsOld");
            return false;
        }

        var users = DataService.LoadUsers();
        var userIndex = users.FindIndex(u => u.Id == CurrentUser.Id);
        if (userIndex < 0)
        {
            errorMessage = LocalizationService.Instance.GetString("Auth.Error.UserNotFound");
            return false;
        }

        users[userIndex].PasswordHash = newHash;
        DataService.SaveUsers(users);
        CurrentUser.PasswordHash = newHash;
        return true;
    }

    /// <summary>
    /// 通知 AuthService 当前用户的角色已在外部被修改（例如 Admin 面板改角色），
    /// 触发一次 <see cref="NotifyRoleChanged"/> 广播以刷新全局 UI
    /// </summary>
    public void NotifyRoleChanged(UserModel updatedUser)
    {
        // 如果修改的是当前登录用户，同步更新内存中的对象
        if (CurrentUser?.Id == updatedUser.Id)
            CurrentUser = updatedUser;
    }

    #endregion

    
}
