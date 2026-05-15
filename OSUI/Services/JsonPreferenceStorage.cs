using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace OSUI.Services;

/// <summary>
/// 基于单个 JSON 文件的偏好设置存储实现。
/// </summary>
public class JsonPreferenceStorage : IPreferenceStorage
{
    private const string PreferencesFileName = "preferences.json";
    private static readonly Object FileLock = new();
    
    private static readonly string DataDir = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Data"
    );
    
    private static string PreferenceFilePath => Path.Combine(DataDir, PreferencesFileName);

    /// <summary>
    /// 读取字符串偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="defaultValue">未命中时返回的默认值。</param>
    /// <returns>读取到的字符串值。</returns>
    public string Get(string key, string defaultValue)
    {
        lock (FileLock)
        {
            var preferences = LoadPreferences();
            return preferences.TryGetValue(key, out var value) && value is not null
                ? value
                : defaultValue;
        }
    }

    /// <summary>
    /// 保存日期时间偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="value">日期时间值。</param>
    public void Set(string key, DateTime value)
    {
        Set(key, value.ToString("O", CultureInfo.InvariantCulture));
    }
    
    /// <summary>
    /// 保存整型偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="value">整型值。</param>
    public void Set(string key, int value)
    {
        Set(key, value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// 读取整型偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="defaultValue">未命中或解析失败时返回的默认值。</param>
    /// <returns>读取到的整型值。</returns>
    public int Get(string key, int defaultValue)
    {
        lock (FileLock)
        {
            var preferences = LoadPreferences();
            return preferences.TryGetValue(key, out var value)
                   && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : defaultValue;
        }
    }

    /// <summary>
    /// 保存字符串偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="value">字符串值。</param>
    public void Set(string key, string value)
    {
        lock (FileLock)
        {
            var preferences = LoadPreferences();
            preferences[key] = value;
            SavePreferences(preferences);
        }
    }


    /// <summary>
    /// 读取日期时间偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="defaultValue">未命中或解析失败时返回的默认值。</param>
    /// <returns>读取到的日期时间值。</returns>
    public DateTime Get(string key, DateTime defaultValue)
    {
        lock (FileLock)
        {
            var preferences = LoadPreferences();
            if (preferences.TryGetValue(key, out var value)
                && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            {
                return parsed;
            }

            return defaultValue;
        }
    }
    
    

    private static Dictionary<string, string> LoadPreferences()
    {
        if (!File.Exists(PreferenceFilePath))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        try
        {
            var json = File.ReadAllText(PreferenceFilePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return data ?? new Dictionary<string, string>(StringComparer.Ordinal);
        }
        catch
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
    }

    private static void SavePreferences(Dictionary<string, string> preferences)
    {
        var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(PreferenceFilePath, json);
    }
}