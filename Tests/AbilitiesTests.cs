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
[AllureSuite("Abilities API")]
[AllureFeature("Abilities Management")]
public class AbilitiesTests : BaseTest
{
    private AbilityResponse _testAbility = null!;
    private string _createdAbilityId = null!;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _testAbility = TestDataGenerator.GenerateTestAbility();
        LogTestData("Сгенерированная тестовая способность", _testAbility);
    }

    [TearDown]
    public async Task TearDown()
    {
        if (!string.IsNullOrEmpty(_createdAbilityId))
        {
            LogTestStep($"Очистка: удаление созданной способности с ID {_createdAbilityId}");
            await AbilitiesClient.DeleteAbilityAsync(_createdAbilityId);
        }
    }

    [Test]
    [AllureTag("API", "Smoke", "Read")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Получение списка всех способностей")]
    public async Task GetAllAbilities_ShouldReturnListOfAbilities()
    {
        // Act
        LogTestStep("Вызов API для получения списка всех способностей");
        var abilities = await ExecuteWithAllureStep(
            "Получение списка способностей",
            () => AbilitiesClient.GetAllAbilitiesAsync()
        );

        // Assert
        LogTestStep("Проверка ответа от API");
        LogTestData("Полученный список способностей", abilities);

        abilities.Should().NotBeNull();
        TestLogger.LogAssertion("Response should not be null");

        abilities.Should().BeOfType<List<AbilityResponse>>();
        TestLogger.LogAssertion("Response should be a List<AbilityResponse>");

        abilities.Count.Should().BeGreaterThan(0);
        TestLogger.LogAssertion($"List should contain at least 1 ability, actual count: {abilities.Count}");

        LogTestStep("Проверка структуры первой способности в списке");
        var firstAbility = abilities.First();
        firstAbility.Should().NotBeNull();
        firstAbility.Id.Should().NotBeNullOrEmpty();
        firstAbility.Name.Should().NotBeNullOrEmpty();
        TestLogger.LogAssertion("First ability should have valid ID and Name");
    }

    [Test]
    [AllureTag("API", "Read")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Получение способности по валидному ID")]
    public async Task GetAbilityById_WithValidId_ShouldReturnAbility()
    {
        // Arrange
        LogTestStep("Получение списка всех способностей для выбора валидного ID");
        var allAbilities = await AbilitiesClient.GetAllAbilitiesAsync();
        var firstAbility = allAbilities.First();
        LogTestData("Первая способность из списка", firstAbility);

        // Act
        LogTestStep($"Вызов API для получения способности по ID: {firstAbility.Id}");
        var ability = await ExecuteWithAllureStep(
            $"Получение способности по ID {firstAbility.Id}",
            () => AbilitiesClient.GetAbilityByIdAsync(firstAbility.Id)
        );

        // Assert
        LogTestStep("Проверка полученной способности");
        LogTestData("Полученная способность по ID", ability);

        ability.Should().NotBeNull();
        TestLogger.LogAssertion("Ability should not be null");

        ability.Id.Should().Be(firstAbility.Id);
        TestLogger.LogAssertion($"Ability ID should match requested ID: {firstAbility.Id}");

        ability.Name.Should().NotBeNullOrEmpty();
        TestLogger.LogAssertion("Ability name should not be empty");

        ability.DangerLevel.Should().BeInRange(1, 10);
        TestLogger.LogAssertion($"Danger level should be between 1-10, actual: {ability.DangerLevel}");
    }

    [Test]
    [AllureTag("API", "Smoke", "Create")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Создание новой способности с валидными данными")]
    public async Task CreateAbility_WithValidData_ShouldCreateAbility()
    {
        // Act
        LogTestStep("Вызов API для создания новой способности");
        var createdAbility = await ExecuteWithAllureStep(
            "Создание новой способности",
            () => AbilitiesClient.CreateAbilityAsync(_testAbility)
        );
        _createdAbilityId = createdAbility.Id;

        // Assert
        LogTestStep("Проверка созданной способности");
        LogTestData("Созданная способность", createdAbility);

        createdAbility.Should().NotBeNull();
        TestLogger.LogAssertion("Created ability should not be null");

        createdAbility.Id.Should().NotBeNullOrEmpty();
        TestLogger.LogAssertion($"Ability should have non-empty ID: {createdAbility.Id}");

        createdAbility.Name.Should().Be(_testAbility.Name);
        TestLogger.LogAssertion($"Name should match: {_testAbility.Name}");

        createdAbility.DangerLevel.Should().BeInRange(1, 10);
        TestLogger.LogAssertion($"Danger level should be in range 1-10, actual: {createdAbility.DangerLevel}");

        createdAbility.Type.Should().BeOneOf("offensive", "defensive", "support", "special");
        TestLogger.LogAssertion($"Type should be valid, actual: {createdAbility.Type}");

        LogTestStep("Верификация: получение созданной способности по ID");
        var retrievedAbility = await AbilitiesClient.GetAbilityByIdAsync(createdAbility.Id);
        retrievedAbility.Id.Should().Be(createdAbility.Id);
        TestLogger.LogAssertion("Retrieved ability should have matching ID");
    }

    [Test]
    [AllureTag("API", "Update")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.normal)]
    [Description("Обновление данных способности")]
    public async Task UpdateAbility_WithValidData_ShouldUpdateAbility()
    {
        // Arrange
        LogTestStep("Создание тестовой способности для обновления");
        var createdAbility = await AbilitiesClient.CreateAbilityAsync(_testAbility);
        _createdAbilityId = createdAbility.Id;
        LogTestData("Исходная способность", createdAbility);

        var updateData = new { DangerLevel = 8, Type = "offensive" };
        LogTestData("Данные для обновления", updateData);

        // Act
        LogTestStep($"Вызов API для обновления способности с ID: {createdAbility.Id}");
        var updatedAbility = await ExecuteWithAllureStep(
            $"Обновление способности {createdAbility.Id}",
            () => AbilitiesClient.UpdateAbilityAsync(createdAbility.Id, updateData)
        );

        // Assert
        LogTestStep("Проверка обновленной способности");
        LogTestData("Обновленная способность", updatedAbility);

        updatedAbility.Should().NotBeNull();
        TestLogger.LogAssertion("Updated ability should not be null");

        updatedAbility.DangerLevel.Should().Be(8);
        TestLogger.LogAssertion($"Danger level should be updated to 8, actual: {updatedAbility.DangerLevel}");

        updatedAbility.Type.Should().Be("offensive");
        TestLogger.LogAssertion($"Type should be updated to 'offensive', actual: {updatedAbility.Type}");

        updatedAbility.Id.Should().Be(createdAbility.Id);
        TestLogger.LogAssertion("ID should remain unchanged");

        LogTestStep("Проверка, что другие поля не изменились");
        updatedAbility.Name.Should().Be(createdAbility.Name);
        TestLogger.LogAssertion("Name should remain unchanged");
    }

    [Test]
    [AllureTag("API", "Delete")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Удаление способности по валидному ID")]
    public async Task DeleteAbility_WithValidId_ShouldDeleteAbility()
    {
        // Arrange
        LogTestStep("Создание тестовой способности для удаления");
        var createdAbility = await AbilitiesClient.CreateAbilityAsync(_testAbility);
        var abilityId = createdAbility.Id;
        LogTestData("Созданная способность для удаления", createdAbility);

        // Act
        LogTestStep($"Вызов API для удаления способности с ID: {abilityId}");
        var deleteResult = await ExecuteWithAllureStep(
            $"Удаление способности {abilityId}",
            () => AbilitiesClient.DeleteAbilityAsync(abilityId)
        );

        // Assert
        LogTestStep("Проверка результата удаления");
        deleteResult.Should().BeTrue();
        TestLogger.LogAssertion("Delete operation should return true");

        LogTestStep("Верификация: проверка что способность больше не доступна");
        var act = async () => await AbilitiesClient.GetAbilityByIdAsync(abilityId);

        await act.Should().ThrowAsync<HttpRequestException>();
        TestLogger.LogAssertion("Getting deleted ability should throw HttpRequestException");

        _createdAbilityId = null!;
        LogTestStep("Очистка: отметка что способность уже удалена");
    }

    [Test]
    [AllureTag("API", "ErrorHandling")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.normal)]
    [Description("Попытка получения несуществующей способности")]
    public async Task GetAbilityById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = "invalid-ability-id-999";
        LogTestStep($"Попытка получения способности с невалидным ID: {invalidId}");

        // Act & Assert
        LogTestStep("Ожидание исключения HttpRequestException для невалидного ID");
        var act = async () => await ExecuteWithAllureStep(
            $"Попытка получения несуществующей способности {invalidId}",
            () => AbilitiesClient.GetAbilityByIdAsync(invalidId)
        );

        await act.Should().ThrowAsync<HttpRequestException>();
        TestLogger.LogAssertion("Should throw HttpRequestException for invalid ability ID");
    }

    [Test]
    [AllureTag("API", "Create")]
    [AllureOwner("QA Team")]
    [AllureSeverity(SeverityLevel.minor)]
    [Description("Создание способности с минимальными данными")]
    public async Task CreateAbility_WithMinimalData_ShouldCreateAbility()
    {
        // Arrange
        LogTestStep("Создание способности только с обязательными полями");
        var minimalAbility = new AbilityResponse
        {
            Name = "Minimal Ability",
            NameEn = "Minimal Ability EN",
            Description = "Minimal description",
            DangerLevel = 1,
            Type = "special"
        };
        LogTestData("Минимальная способность", minimalAbility);

        // Act
        LogTestStep("Вызов API для создания способности с минимальными данными");
        var createdAbility = await ExecuteWithAllureStep(
            "Создание способности с минимальными данными",
            () => AbilitiesClient.CreateAbilityAsync(minimalAbility)
        );
        _createdAbilityId = createdAbility.Id;

        // Assert
        LogTestStep("Проверка созданной способности");
        createdAbility.Should().NotBeNull();
        createdAbility.Id.Should().NotBeNullOrEmpty();
        createdAbility.Name.Should().Be("Minimal Ability");
        TestLogger.LogAssertion("Ability with minimal data should be created successfully");
    }
}