namespace OSUI.Models;

/// <summary>
/// 用户模型，对应 users.json 中的一条记录
/// </summary>
public class UserModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Username { get; init; } = string.Empty;

    /// <summary>SHA-256 哈希后的密码（十六进制字符串）。</summary>
    public string PasswordHash { get; set; } = string.Empty;


    public override string ToString() => $"{Username} ({Id})";
}