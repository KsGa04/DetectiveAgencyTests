using NUnit.Framework;
using System.Text;

namespace DetectiveAgency.Tests.Utilities;

public static class TestReportGenerator
{
    public static void GenerateHtmlReport()
    {
        var reportContent = GenerateReportContent();
        var reportPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestReport.html");

        File.WriteAllText(reportPath, reportContent);
        TestContext.WriteLine($"📊 HTML Report generated: {reportPath}");
    }

    private static string GenerateReportContent()
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='ru'>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='UTF-8'>");
        sb.AppendLine("    <title>Detective Agency API Test Report</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; }");
        sb.AppendLine("        .test { border: 1px solid #ddd; margin: 10px 0; padding: 15px; }");
        sb.AppendLine("        .passed { background-color: #d4edda; }");
        sb.AppendLine("        .failed { background-color: #f8d7da; }");
        sb.AppendLine("        .test-name { font-weight: bold; font-size: 1.2em; }");
        sb.AppendLine("        .test-description { color: #666; margin: 5px 0; }");
        sb.AppendLine("        .log-entry { margin: 2px 0; font-family: monospace; font-size: 0.9em; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($"<h1>📋 Detective Agency API Test Report</h1>");
        sb.AppendLine($"<p>Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");

        // Здесь можно добавить сбор информации о выполненных тестах
        // В реальном проекте это можно интегрировать с NUnit TestContext

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}