using NUnit.Framework;
using System.Text;

namespace DetectiveAgency.Tests.Utilities;

public static class TestLogger
{
    private static readonly StringBuilder _logBuilder = new();
    private static string _currentTestName = string.Empty;

    public static void StartTest(string testName)
    {
        _currentTestName = testName;
        _logBuilder.Clear();
        LogInfo($"🚀 Starting test: {testName}");
        LogInfo($"⏰ Start time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        LogInfo($"📁 Working directory: {ProjectPaths.ProjectRoot}");
    }

    public static void LogInfo(string message)
    {
        var logEntry = $"[INFO] {DateTime.Now:HH:mm:ss.fff} - {message}";
        _logBuilder.AppendLine(logEntry);
        TestContext.WriteLine(logEntry);
    }

    public static void LogRequest(string method, string endpoint, object? body = null)
    {
        LogInfo($"📤 REQUEST: {method} {endpoint}");
        if (body != null)
        {
            var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body, Newtonsoft.Json.Formatting.Indented);
            LogInfo($"📦 REQUEST BODY:\n{jsonBody}");
        }
    }

    public static void LogResponse(string statusCode, object? response = null)
    {
        LogInfo($"📥 RESPONSE: {statusCode}");
        if (response != null)
        {
            var jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            LogInfo($"📨 RESPONSE BODY:\n{jsonResponse}");
        }
    }

    public static void LogAssertion(string assertion)
    {
        LogInfo($"✅ ASSERTION: {assertion}");
    }

    public static void LogError(string error)
    {
        var logEntry = $"[ERROR] {DateTime.Now:HH:mm:ss.fff} - {error}";
        _logBuilder.AppendLine(logEntry);
        TestContext.WriteLine(logEntry);
    }

    public static void EndTest(string status = "COMPLETED")
    {
        LogInfo($"🏁 Test {status}: {_currentTestName}");
        LogInfo($"⏰ End time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        // Сохраняем лог в правильную папку
        SaveLogToFile();
    }

    private static void SaveLogToFile()
    {
        try
        {
            ProjectPaths.EnsureDirectoriesExist();

            var fileName = $"{_currentTestName}_{DateTime.Now:yyyyMMdd_HHmmss}.log";
            var filePath = Path.Combine(ProjectPaths.TestLogs, fileName);

            File.WriteAllText(filePath, _logBuilder.ToString());

            var relativePath = ProjectPaths.GetRelativePath(filePath);
            TestContext.WriteLine($"💾 Log saved to: {relativePath}");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"❌ Failed to save log: {ex.Message}");
        }
    }

    public static string GetTestLog()
    {
        return _logBuilder.ToString();
    }
}