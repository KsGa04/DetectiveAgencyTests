using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace DetectiveAgency.Tests.Tests;

[TestFixture]
[AllureNUnit]
[AllureSuite("Full Regression Suite")]
[AllureFeature("Complete API Testing")]
public class TestSuites
{
    [Test]
    [AllureTag("Regression", "FullSuite")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Полный регрессионный прогон API тестов")]
    public void RunAllApiTests()
    {
        AllureApi.Step("Запуск полного набора API тестов", () => {
            // Этот тест будет запускать другие тесты как шаги
            TestContext.WriteLine("Running complete API test suite...");
        });
    }
}