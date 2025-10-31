using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using DetectiveAgency.Tests.Models.Responses;
using DetectiveAgency.Tests.Utilities;
using FluentAssertions;
using NUnit.Framework;

namespace DetectiveAgency.Tests.Tests;

[TestFixture]
[AllureNUnit]
[AllureSuite("Detectives API")]
[AllureFeature("Detectives Management")]
[AllureSubSuite("CRUD Operations")]
public class DetectivesTests : BaseTest
{
    private DetectiveResponse _testDetective = null!;
    private string _createdDetectiveId = null!;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _testDetective = TestDataGenerator.GenerateTestDetective();
        LogTestData("Сгенерированный тестовый детектив", _testDetective);
    }

    [TearDown]
    public override async Task TearDown()
    {
        if (!string.IsNullOrEmpty(_createdDetectiveId))
        {
            LogTestStep($"Очистка: удаление созданного детектива с ID {_createdDetectiveId}");
            await DetectivesClient.DeleteDetectiveAsync(_createdDetectiveId);
        }

        await base.TearDown();
    }

    [Test]
    [AllureTag("API", "Smoke", "Read")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Получение списка всех детективов")]
    public async Task GetAllDetectives_ShouldReturnListOfDetectives()
    {
        await AllureApi.Step("Получение списка детективов", async () =>
        {
            // Act
            LogTestStep("Вызов API для получения списка детективов");
            var detectives = await DetectivesClient.GetAllDetectivesAsync();

            // Assert
            LogTestStep("Проверка ответа");
            LogTestData("Полученный список детективов", detectives);

            detectives.Should().NotBeNull();
            detectives.Should().BeOfType<List<DetectiveResponse>>();
            TestLogger.LogAssertion("Response should be a List<DetectiveResponse>");

            detectives.Count.Should().BeGreaterThan(0);
            TestLogger.LogAssertion($"List should contain at least 1 detective, actual count: {detectives.Count}");

            LogTestStep("Проверка структуры первого детектива в списке");
            var firstDetective = detectives.First();
            firstDetective.Should().HaveValidDetectiveProperties();
            TestLogger.LogAssertion("First detective should have all required properties");
        });
    }

    [Test]
    [AllureTag("API", "Smoke", "Create")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Создание нового детектива с валидными данными")]
    public async Task CreateDetective_WithValidData_ShouldCreateDetective()
    {
        await AllureApi.Step("Создание нового детектива", async () =>
        {
            // Act
            LogTestStep("Вызов API для создания нового детектива");
            var createdDetective = await DetectivesClient.CreateDetectiveAsync(_testDetective);
            _createdDetectiveId = createdDetective.Id;

            // Assert
            LogTestStep("Проверка созданного детектива");
            LogTestData("Созданный детектив", createdDetective);

            createdDetective.Should().NotBeNull();
            createdDetective.Id.Should().NotBeNullOrEmpty();
            TestLogger.LogAssertion($"Detective should have non-empty ID: {createdDetective.Id}");

            createdDetective.Name.Should().Be(_testDetective.Name);
            TestLogger.LogAssertion($"Name should be '{_testDetective.Name}'");

            createdDetective.Status.Should().Be("active");
            TestLogger.LogAssertion("Status should be 'active'");

            createdDetective.Age.Should().Be(_testDetective.Age);
            TestLogger.LogAssertion($"Age should be {_testDetective.Age}");

            LogTestStep("Верификация: получение созданного детектива по ID");
            var retrievedDetective = await DetectivesClient.GetDetectiveByIdAsync(createdDetective.Id);
            retrievedDetective.Id.Should().Be(createdDetective.Id);
            TestLogger.LogAssertion("Retrieved detective should have matching ID");
        });
    }

    [Test]
    [AllureTag("API", "Read")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Получение детектива по валидному ID")]
    public async Task GetDetectiveById_WithValidId_ShouldReturnDetective()
    {
        await AllureApi.Step("Получение детектива по ID", async () =>
        {
            // Arrange
            LogTestStep("Получение списка всех детективов для выбора валидного ID");
            var allDetectives = await DetectivesClient.GetAllDetectivesAsync();
            var firstDetective = allDetectives.First();
            LogTestData("Первый детектив из списка", firstDetective);

            // Act
            LogTestStep($"Вызов API для получения детектива по ID: {firstDetective.Id}");
            var detective = await DetectivesClient.GetDetectiveByIdAsync(firstDetective.Id);

            // Assert
            LogTestStep("Проверка полученного детектива");
            LogTestData("Полученный детектив по ID", detective);

            detective.Should().NotBeNull();
            TestLogger.LogAssertion("Detective should not be null");

            detective.Id.Should().Be(firstDetective.Id);
            TestLogger.LogAssertion($"Detective ID should match requested ID: {firstDetective.Id}");

            detective.Name.Should().NotBeNullOrEmpty();
            TestLogger.LogAssertion("Detective name should not be empty");

            detective.Status.Should().BeOneOf("active", "inactive");
            TestLogger.LogAssertion($"Status should be valid, actual: {detective.Status}");

            detective.Age.Should().BeGreaterThan(0);
            TestLogger.LogAssertion($"Age should be positive, actual: {detective.Age}");
        });
    }

    [Test]
    [AllureTag("API", "Update")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("Обновление данных детектива")]
    public async Task UpdateDetective_WithValidData_ShouldUpdateDetective()
    {
        await AllureApi.Step("Обновление детектива", async () =>
        {
            // Arrange
            LogTestStep("Создание тестового детектива для обновления");
            var createdDetective = await DetectivesClient.CreateDetectiveAsync(_testDetective);
            _createdDetectiveId = createdDetective.Id;
            LogTestData("Исходный детектив", createdDetective);

            var updateData = new { Status = "inactive", Age = 35 };
            LogTestData("Данные для обновления", updateData);

            // Act
            LogTestStep($"Вызов API для обновления детектива с ID: {createdDetective.Id}");
            var updatedDetective = await DetectivesClient.UpdateDetectiveAsync(createdDetective.Id, updateData);

            // Assert
            LogTestStep("Проверка обновленного детектива");
            LogTestData("Обновленный детектив", updatedDetective);

            updatedDetective.Should().NotBeNull();
            TestLogger.LogAssertion("Updated detective should not be null");

            updatedDetective.Status.Should().Be("inactive");
            TestLogger.LogAssertion($"Status should be updated to 'inactive', actual: {updatedDetective.Status}");

            updatedDetective.Age.Should().Be(35);
            TestLogger.LogAssertion($"Age should be updated to 35, actual: {updatedDetective.Age}");

            updatedDetective.Id.Should().Be(createdDetective.Id);
            TestLogger.LogAssertion("ID should remain unchanged");

            LogTestStep("Проверка, что другие поля не изменились");
            updatedDetective.Name.Should().Be(createdDetective.Name);
            TestLogger.LogAssertion("Name should remain unchanged");
        });
    }

    [Test]
    [AllureTag("API", "Delete")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Удаление детектива по валидному ID")]
    public async Task DeleteDetective_WithValidId_ShouldDeleteDetective()
    {
        await AllureApi.Step("Удаление детектива", async () =>
        {
            // Arrange
            LogTestStep("Создание тестового детектива для удаления");
            var createdDetective = await DetectivesClient.CreateDetectiveAsync(_testDetective);
            var detectiveId = createdDetective.Id;
            LogTestData("Созданный детектив для удаления", createdDetective);

            // Act
            LogTestStep($"Вызов API для удаления детектива с ID: {detectiveId}");
            var deleteResult = await DetectivesClient.DeleteDetectiveAsync(detectiveId);

            // Assert
            LogTestStep("Проверка результата удаления");
            deleteResult.Should().BeTrue();
            TestLogger.LogAssertion("Delete operation should return true");

            LogTestStep("Верификация: проверка что детектив больше не доступен");
            var act = async () => await DetectivesClient.GetDetectiveByIdAsync(detectiveId);

            await act.Should().ThrowAsync<HttpRequestException>();
            TestLogger.LogAssertion("Getting deleted detective should throw HttpRequestException");

            _createdDetectiveId = null!;
            LogTestStep("Очистка: отметка что детектив уже удален");
        });
    }

    [Test]
    [AllureTag("API", "ErrorHandling")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("Попытка получения несуществующего детектива")]
    public async Task GetDetectiveById_WithInvalidId_ShouldReturnNotFound()
    {
        await AllureApi.Step("Попытка получения несуществующего детектива", async () =>
        {
            // Arrange
            var invalidId = "invalid-detective-id-999";
            LogTestStep($"Попытка получения детектива с невалидным ID: {invalidId}");

            // Act & Assert
            LogTestStep("Ожидание исключения HttpRequestException для невалидного ID");
            var act = async () => await DetectivesClient.GetDetectiveByIdAsync(invalidId);

            await act.Should().ThrowAsync<HttpRequestException>();
            TestLogger.LogAssertion("Should throw HttpRequestException for invalid detective ID");
        });
    }
}