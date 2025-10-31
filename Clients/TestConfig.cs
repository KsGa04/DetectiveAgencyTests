using System.Text.Json;

namespace DetectiveAgency.Tests.Config;

public class TestConfig
{
    private readonly JsonDocument _config;

    public TestConfig()
    {
        // Ищем appsettings.json в разных возможных местах
        var configPath = FindConfigFile();
        if (string.IsNullOrEmpty(configPath))
            throw new FileNotFoundException("appsettings.json not found. Please ensure the file exists in the project root and is set to 'Copy if newer'.");

        var json = File.ReadAllText(configPath);
        _config = JsonDocument.Parse(json);
    }

    public string BaseUrl => GetString("BaseUrl") ??
        throw new InvalidOperationException("BaseUrl is not configured");

    public string AdminUsername => GetString("Auth:AdminUsername") ?? "admin";
    public string AdminPassword => GetString("Auth:AdminPassword") ?? "admin123";

    public int TimeoutSeconds => GetInt("TestSettings:TimeoutSeconds") ?? 30;
    public int RetryCount => GetInt("TestSettings:RetryCount") ?? 3;

    private string? FindConfigFile()
    {
        // Пробуем разные возможные расположения файла
        var possiblePaths = new[]
        {
            "appsettings.json", // Текущая директория
            Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"),
            Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
            Path.Combine(GetProjectRoot(), "appsettings.json")
        };

        return possiblePaths.FirstOrDefault(File.Exists);
    }

    private string GetProjectRoot()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDir);

        // Поднимаемся вверх по директориям, пока не найдем файл проекта
        while (directory != null && !directory.GetFiles("*.csproj").Any())
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? currentDir;
    }

    private string? GetString(string key)
    {
        try
        {
            var parts = key.Split(':');
            JsonElement current = _config.RootElement;

            foreach (var part in parts)
            {
                if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var property))
                    current = property;
                else
                    return null;
            }

            return current.ValueKind == JsonValueKind.String ? current.GetString() : null;
        }
        catch
        {
            return null;
        }
    }

    private int? GetInt(string key)
    {
        try
        {
            var stringValue = GetString(key);
            return int.TryParse(stringValue, out int result) ? result : null;
        }
        catch
        {
            return null;
        }
    }
}