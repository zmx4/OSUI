using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OSUI.Models;

namespace OSUI.Services;

public static class DataService
{
    private static readonly string DataDir = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Data"
    );

    private static readonly string UsersFile = Path.Combine(DataDir, "users.json");
    // private static readonly string PostsFile = Path.Combine(DataDir, "posts.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    #region Users

    public static List<UserModel> LoadUsers()
    {
        if (!File.Exists(UsersFile))
            return [];
        var json = File.ReadAllText(UsersFile);
        return JsonSerializer.Deserialize<List<UserModel>>(json, JsonOptions) ?? [];
    }

    public static void SaveUsers(IEnumerable<UserModel> users)
    {
        Directory.CreateDirectory(DataDir);
        var json = JsonSerializer.Serialize(users, JsonOptions);
        File.WriteAllText(UsersFile, json);
    }

    #endregion
    
}
