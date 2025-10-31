using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using DetectiveAgency.Tests.Models.Requests;
using FluentAssertions;
using NUnit.Framework;

namespace DetectiveAgency.Tests.Tests;

[TestFixture]
[AllureNUnit]
[AllureSuite("Authentication")]
[AllureFeature("User Authentication")]
public class AuthTests : BaseTest
{
    [Test]
    [AllureTag("API", "Smoke", "Authentication")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("AUTH-001")]
    [AllureTms("TMS-001")]
    [AllureDescription("Тестирование успешной авторизации с валидными учетными данными администратора")]
    public async Task Login_WithValidCredentials_ShouldReturnSuccess()
    {
        await AllureApi.Step("Подготовка тестовых данных", async () =>
        {
            // Arrange
            LogTestStep("Подготовка тестовых данных - валидные учетные данные администратора");
            var credentials = new LoginCredentials
            {
                Username = Config.AdminUsername,
                Password = Config.AdminPassword
            };
            LogTestData("Учетные данные", credentials);

            // Act
            LogTestStep("Вызов API авторизации");
            var response = await AuthClient.LoginAsync(credentials);

            // Assert
            LogTestStep("Проверка ответа от API");
            LogTestData("Ответ авторизации", response);

            response.Message.Should().Be("Login successful");
            AllureApi.Step("Проверка сообщения об успехе", () => {
                TestLogger.LogAssertion("Message should be 'Login successful'");
            });

            response.User.Should().NotBeNull();
            response.User.Username.Should().Be(Config.AdminUsername);
            response.User.Role.Should().Be("admin");

            AllureApi.Step("Проверка данных пользователя", () => {
                TestLogger.LogAssertion($"User should have username '{Config.AdminUsername}' and role 'admin'");
            });
        });
    }

    [Test]
    [AllureTag("API", "Security")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("Тестирование авторизации с невалидными учетными данными")]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        LogTestStep("Подготовка тестовых данных - невалидные учетные данные");
        var credentials = new LoginCredentials
        {
            Username = "invalid_user",
            Password = "wrong_password_123"
        };
        LogTestData("Невалидные учетные данные", credentials);

        // Act & Assert
        LogTestStep("Попытка авторизации с невалидными данными и проверка исключения");
        var act = async () => await AuthClient.LoginAsync(credentials);

        LogTestStep("Проверка, что выбрасывается исключение HttpRequestException");
        await act.Should().ThrowAsync<HttpRequestException>();
        TestLogger.LogAssertion("Should throw HttpRequestException for invalid credentials");
    }
}