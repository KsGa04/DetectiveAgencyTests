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
[AllureSuite("Cases API")]
[AllureFeature("Cases Management")]
[AllureSubSuite("CRUD Operations")]
public class CasesTests : BaseTest
{
    private CaseResponse _testCase = null!;
    private string _createdCaseId = null!;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _testCase = TestDataGenerator.GenerateTestCase();
        LogTestData("Сгенерированное тестовое дело", _testCase);
    }

    [TearDown]
    public async Task TearDown()
    {
        if (!string.IsNullOrEmpty(_createdCaseId))
        {
            LogTestStep($"Очистка: удаление созданного дела с ID {_createdCaseId}");
            await CasesClient.DeleteCaseAsync(_createdCaseId);
        }
    }

    [Test]
    [AllureTag("API", "Smoke", "Read")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Получение списка всех дел")]
    public async Task GetAllCases_ShouldReturnListOfCases()
    {
        await AllureApi.Step("Получение списка дел", async () =>
        {
            // Act
            LogTestStep("Вызов API для получения списка всех дел");
            var cases = await CasesClient.GetAllCasesAsync();

            // Assert
            LogTestStep("Проверка ответа от API");
            LogTestData("Полученный список дел", cases);

            cases.Should().NotBeNull();
            TestLogger.LogAssertion("Response should not be null");

            cases.Should().BeOfType<List<CaseResponse>>();
            TestLogger.LogAssertion("Response should be a List<CaseResponse>");

            cases.Count.Should().BeGreaterThan(0);
            TestLogger.LogAssertion($"List should contain at least 1 case, actual count: {cases.Count}");

            LogTestStep("Проверка структуры первого дела в списке");
            var firstCase = cases.First();
            firstCase.Should().HaveValidCaseProperties();
            TestLogger.LogAssertion("First case should have all required properties");
        });
    }

    [Test]
    [AllureTag("API", "Read")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Получение дела по валидному ID")]
    public async Task GetCaseById_WithValidId_ShouldReturnCase()
    {
        await AllureApi.Step("Получение дела по ID", async () =>
        {
            // Arrange
            LogTestStep("Получение списка всех дел для выбора валидного ID");
            var allCases = await CasesClient.GetAllCasesAsync();
            var firstCase = allCases.First();
            LogTestData("Первое дело из списка", firstCase);

            // Act
            LogTestStep($"Вызов API для получения дела по ID: {firstCase.Id}");
            var caseEntity = await CasesClient.GetCaseByIdAsync(firstCase.Id);

            // Assert
            LogTestStep("Проверка полученного дела");
            LogTestData("Полученное дело по ID", caseEntity);

            caseEntity.Should().NotBeNull();
            TestLogger.LogAssertion("Case should not be null");

            caseEntity.Id.Should().Be(firstCase.Id);
            TestLogger.LogAssertion($"Case ID should match requested ID: {firstCase.Id}");

            caseEntity.Title.Should().NotBeNullOrEmpty();
            TestLogger.LogAssertion("Case title should not be empty");

            caseEntity.Status.Should().BeOneOf("open", "in-progress", "closed");
            TestLogger.LogAssertion($"Status should be valid, actual: {caseEntity.Status}");

            caseEntity.Priority.Should().BeOneOf("low", "medium", "high", "critical");
            TestLogger.LogAssertion($"Priority should be valid, actual: {caseEntity.Priority}");
        });
    }

    [Test]
    [AllureTag("API", "Smoke", "Create")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Создание нового дела с валидными данными")]
    public async Task CreateCase_WithValidData_ShouldCreateCase()
    {
        await AllureApi.Step("Создание нового дела", async () =>
        {
            // Act
            LogTestStep("Вызов API для создания нового дела");
            var createdCase = await CasesClient.CreateCaseAsync(_testCase);
            _createdCaseId = createdCase.Id;

            // Assert
            LogTestStep("Проверка созданного дела");
            LogTestData("Созданное дело", createdCase);

            createdCase.Should().NotBeNull();
            TestLogger.LogAssertion("Created case should not be null");

            createdCase.Id.Should().NotBeNullOrEmpty();
            TestLogger.LogAssertion($"Case should have non-empty ID: {createdCase.Id}");

            createdCase.Title.Should().Be(_testCase.Title);
            TestLogger.LogAssertion($"Title should match: {_testCase.Title}");

            // Исправлено: API сохраняет переданный статус, а не всегда "open"
            createdCase.Status.Should().Be(_testCase.Status);
            TestLogger.LogAssertion($"Status should match the one sent: {_testCase.Status}");

            createdCase.CreatedAt.Should().NotBeNullOrEmpty();
            TestLogger.LogAssertion("CreatedAt should not be empty");

            createdCase.UpdatedAt.Should().NotBeNullOrEmpty();
            TestLogger.LogAssertion("UpdatedAt should not be empty");

            LogTestStep("Верификация: получение созданного дела по ID");
            var retrievedCase = await CasesClient.GetCaseByIdAsync(createdCase.Id);
            retrievedCase.Id.Should().Be(createdCase.Id);
            TestLogger.LogAssertion("Retrieved case should have matching ID");
        });
    }

    [Test]
    [AllureTag("API", "Update")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("Обновление данных дела")]
    public async Task UpdateCase_WithValidData_ShouldUpdateCase()
    {
        await AllureApi.Step("Обновление дела", async () =>
        {
            // Arrange
            LogTestStep("Создание тестового дела для обновления");
            var createdCase = await CasesClient.CreateCaseAsync(_testCase);
            _createdCaseId = createdCase.Id;
            LogTestData("Исходное дело", createdCase);

            var updateData = new { Status = "closed", Priority = "high" };
            LogTestData("Данные для обновления", updateData);

            // Act
            LogTestStep($"Вызов API для обновления дела с ID: {createdCase.Id}");
            var updatedCase = await CasesClient.UpdateCaseAsync(createdCase.Id, updateData);

            // Assert
            LogTestStep("Проверка обновленного дела");
            LogTestData("Обновленное дело", updatedCase);

            updatedCase.Should().NotBeNull();
            TestLogger.LogAssertion("Updated case should not be null");

            updatedCase.Status.Should().Be("closed");
            TestLogger.LogAssertion($"Status should be updated to 'closed', actual: {updatedCase.Status}");

            updatedCase.Priority.Should().Be("high");
            TestLogger.LogAssertion($"Priority should be updated to 'high', actual: {updatedCase.Priority}");

            updatedCase.Id.Should().Be(createdCase.Id);
            TestLogger.LogAssertion("ID should remain unchanged");

            updatedCase.UpdatedAt.Should().NotBe(createdCase.UpdatedAt);
            TestLogger.LogAssertion("UpdatedAt should change after update");

            LogTestStep("Проверка, что другие поля не изменились");
            updatedCase.Title.Should().Be(createdCase.Title);
            TestLogger.LogAssertion("Title should remain unchanged");

            updatedCase.Description.Should().Be(createdCase.Description);
            TestLogger.LogAssertion("Description should remain unchanged");
        });
    }

    [Test]
    [AllureTag("API", "Delete")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureDescription("Удаление дела по валидному ID")]
    public async Task DeleteCase_WithValidId_ShouldDeleteCase()
    {
        await AllureApi.Step("Удаление дела", async () =>
        {
            // Arrange
            LogTestStep("Создание тестового дела для удаления");
            var createdCase = await CasesClient.CreateCaseAsync(_testCase);
            var caseId = createdCase.Id;
            LogTestData("Созданное дело для удаления", createdCase);

            // Act
            LogTestStep($"Вызов API для удаления дела с ID: {caseId}");
            var deleteResult = await CasesClient.DeleteCaseAsync(caseId);

            // Assert
            LogTestStep("Проверка результата удаления");
            deleteResult.Should().BeTrue();
            TestLogger.LogAssertion("Delete operation should return true");

            LogTestStep("Верификация: проверка что дело больше не доступно");
            var act = async () => await CasesClient.GetCaseByIdAsync(caseId);

            await act.Should().ThrowAsync<HttpRequestException>();
            TestLogger.LogAssertion("Getting deleted case should throw HttpRequestException");

            _createdCaseId = null!;
            LogTestStep("Очистка: отметка что дело уже удалено");
        });
    }

    [Test]
    [AllureTag("API", "Validation")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription("Создание дела с пустым заголовком - проверка что API это разрешает")]
    public async Task CreateCase_WithEmptyTitle_ShouldBeAllowedByApi()
    {
        await AllureApi.Step("Создание дела с пустым заголовком", async () =>
        {
            // Arrange
            LogTestStep("Создание дела с пустым заголовком для проверки поведения API");
            var caseWithEmptyTitle = new CaseResponse
            {
                Title = "", // Пустой заголовок
                Description = "Test Description",
                Status = "open",
                Priority = "medium",
                AssignedTo = new List<string> { "1" },
                Location = "Test Location"
            };
            LogTestData("Дело с пустым заголовком", caseWithEmptyTitle);

            // Act
            LogTestStep("Вызов API для создания дела с пустым заголовком");
            var createdCase = await CasesClient.CreateCaseAsync(caseWithEmptyTitle);
            _createdCaseId = createdCase.Id;

            // Assert
            LogTestStep("Проверка что API принял дело с пустым заголовком");
            LogTestData("Созданное дело", createdCase);

            createdCase.Should().NotBeNull();
            TestLogger.LogAssertion("Case should be created even with empty title");

            createdCase.Id.Should().NotBeNullOrEmpty();
            TestLogger.LogAssertion("Case should have valid ID");

            createdCase.Title.Should().Be("");
            TestLogger.LogAssertion("Title should remain empty as sent");

            // Дополнительная проверка: убедимся что дело можно получить по ID
            var retrievedCase = await CasesClient.GetCaseByIdAsync(createdCase.Id);
            retrievedCase.Should().NotBeNull();
            TestLogger.LogAssertion("Should be able to retrieve case with empty title");
        });
    }

    [Test]
    [AllureTag("API", "OptionalFields")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.minor)]
    [AllureDescription("Проверка работы с наградой (reward) - опциональное поле")]
    public async Task CreateCase_WithAndWithoutReward_ShouldHandleCorrectly()
    {
        await AllureApi.Step("Проверка опционального поля reward", async () =>
        {
            // Arrange
            LogTestStep("Создание дела с наградой");
            var caseWithReward = TestDataGenerator.GenerateTestCase();
            caseWithReward.Reward = 50000;
            LogTestData("Дело с наградой", caseWithReward);

            // Act
            LogTestStep("Создание дела с указанием награды");
            var createdCase = await CasesClient.CreateCaseAsync(caseWithReward);
            _createdCaseId = createdCase.Id;

            // Assert
            LogTestStep("Проверка что награда сохранилась корректно");
            createdCase.Reward.Should().Be(50000);
            TestLogger.LogAssertion("Reward should be saved correctly");

            LogTestStep("Создание дела без награды");
            var caseWithoutReward = TestDataGenerator.GenerateTestCase();
            caseWithoutReward.Reward = null;

            var createdCaseWithoutReward = await CasesClient.CreateCaseAsync(caseWithoutReward);
            var tempCaseId = createdCaseWithoutReward.Id;

            // Assert для дела без награды
            createdCaseWithoutReward.Reward.Should().BeNull();
            TestLogger.LogAssertion("Reward should be null when not specified");

            // Cleanup второго дела
            await CasesClient.DeleteCaseAsync(tempCaseId);
        });
    }

    [Test]
    [AllureTag("API", "ErrorHandling")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureDescription("Попытка получения несуществующего дела")]
    public async Task GetCaseById_WithInvalidId_ShouldReturnNotFound()
    {
        await AllureApi.Step("Попытка получения несуществующего дела", async () =>
        {
            // Arrange
            var invalidId = "invalid-case-id-999";
            LogTestStep($"Попытка получения дела с невалидным ID: {invalidId}");

            // Act & Assert
            LogTestStep("Ожидание исключения HttpRequestException для невалидного ID");
            var act = async () => await CasesClient.GetCaseByIdAsync(invalidId);

            await act.Should().ThrowAsync<HttpRequestException>();
            TestLogger.LogAssertion("Should throw HttpRequestException for invalid case ID");
        });
    }
}