using DetectiveAgency.Tests.Models.Responses;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace DetectiveAgency.Tests.Utilities;

public static class AssertExtensions
{
    public static AndConstraint<StringAssertions> BeValidIsoDate(
        this StringAssertions stringAssertions,
        string because = "",
        params object[] becauseArgs)
    {
        return stringAssertions.MatchRegex(
            @"^\d{4}-\d{2}-\d{2}(T\d{2}:\d{2}:\d{2}(\.\d+)?Z?)?$",
            because, becauseArgs);
    }

    public static AndConstraint<ObjectAssertions> HaveValidDetectiveProperties(
        this ObjectAssertions objectAssertions,
        string because = "",
        params object[] becauseArgs)
    {
        return objectAssertions.BeOfType<DetectiveResponse>(because, becauseArgs)
            .And.Match<DetectiveResponse>(d =>
                !string.IsNullOrEmpty(d.Id) &&
                !string.IsNullOrEmpty(d.Name) &&
                !string.IsNullOrEmpty(d.Status) &&
                d.Age > 0 &&
                !string.IsNullOrEmpty(d.JoinedAt));
    }

    public static AndConstraint<ObjectAssertions> HaveValidCaseProperties(
        this ObjectAssertions objectAssertions,
        string because = "",
        params object[] becauseArgs)
    {
        return objectAssertions.BeOfType<CaseResponse>(because, becauseArgs)
            .And.Match<CaseResponse>(c =>
                !string.IsNullOrEmpty(c.Id) &&
                !string.IsNullOrEmpty(c.Title) &&
                new[] { "open", "in-progress", "closed" }.Contains(c.Status) &&
                new[] { "low", "medium", "high", "critical" }.Contains(c.Priority) &&
                !string.IsNullOrEmpty(c.CreatedAt));
    }
}