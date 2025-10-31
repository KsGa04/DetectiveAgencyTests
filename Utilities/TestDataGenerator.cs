using DetectiveAgency.Tests.Models.Responses;

namespace DetectiveAgency.Tests.Utilities;

public static class TestDataGenerator
{
    private static readonly Random _random = new();

    public static DetectiveResponse GenerateTestDetective()
    {
        return new DetectiveResponse
        {
            Name = $"Test Detective {Guid.NewGuid()}",
            NameEn = $"Test Detective EN {Guid.NewGuid()}",
            Role = "Детектив",
            Ability = "Test Ability",
            AbilityId = "1",
            Description = "Test Description for detective",
            Image = "/test.jpg",
            Status = "active",
            Age = _random.Next(20, 50),
            JoinedAt = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };
    }

    public static CaseResponse GenerateTestCase()
    {
        var priorities = new[] { "low", "medium", "high", "critical" };
        var statuses = new[] { "open", "in-progress", "closed" };

        return new CaseResponse
        {
            Title = $"Test Case {Guid.NewGuid()}",
            Description = $"Test Case Description {Guid.NewGuid()}",
            Status = statuses[_random.Next(statuses.Length)],
            Priority = priorities[_random.Next(priorities.Length)],
            AssignedTo = new List<string> { "1" },
            Location = $"Test Location {Guid.NewGuid()}",
            Reward = _random.Next(1000, 100000),
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }

    public static AbilityResponse GenerateTestAbility()
    {
        var types = new[] { "offensive", "defensive", "support", "special" };
        var activations = new[] { "Пассивная", "Произвольная", "Автоматическая" };
        var ranges = new[] { "Прикосновение", "10 метров", "50 метров", "Неограниченная" };

        return new AbilityResponse
        {
            Name = $"Test Ability {Guid.NewGuid()}",
            NameEn = $"Test Ability EN {Guid.NewGuid()}",
            Type = types[_random.Next(types.Length)],
            Description = $"Test Ability Description {Guid.NewGuid()}",
            DangerLevel = _random.Next(1, 11),
            Range = ranges[_random.Next(ranges.Length)],
            Activation = activations[_random.Next(activations.Length)]
        };
    }

    public static List<string> GenerateDetectiveIds(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => i.ToString())
            .ToList();
    }
    public static void LogTestDataSummary()
    {
        TestLogger.LogInfo("📈 TEST DATA SUMMARY:");
        TestLogger.LogInfo($"   - Generated detectives: {_random.Next(1, 100)}");
        TestLogger.LogInfo($"   - Generated cases: {_random.Next(1, 50)}");
        TestLogger.LogInfo($"   - Generated abilities: {_random.Next(1, 30)}");
    }

    public static string GenerateTestSummary()
    {
        return $"""
        🧪 TEST EXECUTION SUMMARY
        ⏰ Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
        🔧 Environment: API Testing
        🎯 Target: Detective Agency Management System
        📊 Test Scope: CRUD operations for Detectives, Cases, Abilities
        """;
    }
}