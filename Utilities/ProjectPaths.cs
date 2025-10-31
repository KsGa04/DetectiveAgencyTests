namespace DetectiveAgency.Tests.Utilities;

public static class ProjectPaths
{
    public static string ProjectRoot => FindProjectRoot();
    public static string TestLogs => Path.Combine(ProjectRoot, "TestLogs");
    public static string AllureResults => Path.Combine(ProjectRoot, "allure-results");
    public static string TestResults => Path.Combine(ProjectRoot, "TestResults");
    public static string AllureReport => Path.Combine(ProjectRoot, "allure-report");

    private static string FindProjectRoot()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDir);

        // Поднимаемся вверх, пока не найдем файл .csproj
        while (directory != null && !directory.GetFiles("*.csproj").Any())
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? currentDir;
    }

    public static void EnsureDirectoriesExist()
    {
        Directory.CreateDirectory(TestLogs);
        Directory.CreateDirectory(AllureResults);
        Directory.CreateDirectory(TestResults);
        Directory.CreateDirectory(AllureReport);
    }

    public static string GetRelativePath(string fullPath)
    {
        var projectRoot = new Uri(ProjectRoot + Path.DirectorySeparatorChar);
        var filePath = new Uri(fullPath);
        return Uri.UnescapeDataString(projectRoot.MakeRelativeUri(filePath).ToString());
    }
}