using Allure.Net.Commons;
using DetectiveAgency.Tests.Clients;
using DetectiveAgency.Tests.Config;
using DetectiveAgency.Tests.Utilities;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace DetectiveAgency.Tests.Tests;

[TestFixture]
public abstract class BaseTest
{
    protected TestConfig Config { get; private set; } = null!;
    protected AuthClient AuthClient { get; private set; } = null!;
    protected DetectivesClient DetectivesClient { get; private set; } = null!;
    protected CasesClient CasesClient { get; private set; } = null!;
    protected AbilitiesClient AbilitiesClient { get; private set; } = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Убеждаемся, что все папки созданы
        ProjectPaths.EnsureDirectoriesExist();

        // Устанавливаем переменную окружения для Allure
        Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIR", ProjectPaths.AllureResults);

        Config = new TestConfig();

        AuthClient = new AuthClient(Config.BaseUrl);
        DetectivesClient = new DetectivesClient(Config.BaseUrl);
        CasesClient = new CasesClient(Config.BaseUrl);
        AbilitiesClient = new AbilitiesClient(Config.BaseUrl);

        TestLogger.LogInfo($"🔧 Project root: {ProjectPaths.ProjectRoot}");
        TestLogger.LogInfo($"🔧 Allure results: {ProjectPaths.AllureResults}");
        TestLogger.LogInfo($"🔧 Test logs: {ProjectPaths.TestLogs}");
    }

    [SetUp]
    public virtual void SetUp()
    {
        TestLogger.StartTest(TestContext.CurrentContext.Test.Name);
        TestLogger.LogInfo($"Test Class: {GetType().Name}");
        TestLogger.LogInfo($"Test Description: {TestContext.CurrentContext.Test.Properties.Get("Description") ?? "No description"}");
    }

    [TearDown]
    public virtual void TearDown()
    {
        var testResult = TestContext.CurrentContext.Result.Outcome.Status;
        var status = testResult == TestStatus.Passed ? "PASSED" :
                    testResult == TestStatus.Failed ? "FAILED" : "OTHER";

        if (testResult == TestStatus.Failed)
        {
            TestLogger.LogError($"Test failed: {TestContext.CurrentContext.Result.Message}");
            TestLogger.LogError($"Stack trace: {TestContext.CurrentContext.Result.StackTrace}");
        }

        TestLogger.LogInfo($"Test Result: {status}");
        TestLogger.EndTest(status);

        Thread.Sleep(100);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestLogger.LogInfo(TestDataGenerator.GenerateTestSummary());

        // Генерируем Allure отчет
        GenerateAllureReport();
    }

    protected void LogTestStep(string step)
    {
        TestLogger.LogInfo($"🔹 STEP: {step}");

        // Safe Allure step
        try
        {
            AllureApi.Step(step);
        }
        catch (Exception ex)
        {
            TestLogger.LogError($"Allure step failed: {ex.Message}");
        }
    }

    protected void LogTestData(string dataName, object data)
    {
        var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        TestLogger.LogInfo($"📊 TEST DATA [{dataName}]: {jsonData}");

        // Safe Allure attachment
        try
        {
            AllureApi.AddAttachment(dataName, "application/json", jsonData);
        }
        catch (Exception ex)
        {
            TestLogger.LogError($"Allure attachment failed: {ex.Message}");
        }
    }

    protected async Task<T> ExecuteWithAllureStep<T>(string stepName, Func<Task<T>> action)
    {
        try
        {
            return await AllureApi.Step(stepName, action);
        }
        catch (Exception ex)
        {
            TestLogger.LogError($"Allure step '{stepName}' failed: {ex.Message}");
            return await action();
        }
    }

    private void GenerateAllureReport()
    {
        try
        {
            if (Directory.Exists(ProjectPaths.AllureResults) &&
                Directory.GetFiles(ProjectPaths.AllureResults).Any())
            {
                TestLogger.LogInfo("📊 Generating Allure report...");

                // Используем Process для вызова allure
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "allure",
                        Arguments = $"generate {ProjectPaths.AllureResults} --clean -o {ProjectPaths.AllureReport}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    TestLogger.LogInfo($"✅ Allure report generated: {ProjectPaths.GetRelativePath(ProjectPaths.AllureReport)}");
                }
                else
                {
                    var error = process.StandardError.ReadToEnd();
                    TestLogger.LogError($"❌ Allure report generation failed: {error}");
                }
            }
            else
            {
                TestLogger.LogInfo("ℹ️ No Allure results found to generate report");
            }
        }
        catch (Exception ex)
        {
            TestLogger.LogError($"❌ Allure report generation failed: {ex.Message}");
            TestLogger.LogInfo("💡 Make sure Allure CLI is installed: https://docs.qameta.io/allure/#_installing_a_commandline");
        }
    }
}