namespace OSUI.Services;


/// <summary>
/// 提供应用偏好设置的键值读写能力。
/// </summary>
public interface IPreferenceStorage
{
    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    void Set(string key, int value);

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="defaultValue">值</param>
    int Get(string key, int defaultValue);

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    void Set(string key, string value);

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="defaultValue">值</param>
    string Get(string key, string defaultValue);

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    void Set(string key, DateTime value);

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="defaultValue">值</param>
    DateTime Get(string key, DateTime defaultValue);
}