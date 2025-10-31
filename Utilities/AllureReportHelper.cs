using System.Diagnostics;

namespace DetectiveAgency.Tests.Utilities;

public static class AllureReportHelper
{
    public static void GenerateAllureReport()
    {
        var resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");
        var reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "allure-report");

        if (!Directory.Exists(resultsDirectory))
        {
            Console.WriteLine("No allure-results directory found. Run tests first.");
            return;
        }

        try
        {
            // Используем allure commandline для генерации отчета
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C allure generate {resultsDirectory} -o {reportDirectory} --clean",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            Console.WriteLine("Allure report generated successfully!");
            Console.WriteLine($"Report location: {reportDirectory}\\index.html");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to generate Allure report: {ex.Message}");
            Console.WriteLine("Make sure Allure CLI is installed: https://docs.qameta.io/allure/#_installing_a_commandline");
        }
    }

    public static void OpenAllureReport()
    {
        var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "allure-report", "index.html");

        if (File.Exists(reportPath))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = reportPath,
                UseShellExecute = true
            });
        }
        else
        {
            Console.WriteLine("Allure report not found. Generate it first.");
        }
    }
}