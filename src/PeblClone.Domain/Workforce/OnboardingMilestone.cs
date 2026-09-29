namespace PeblClone.Domain.Workforce;

public sealed record OnboardingMilestone(
    string Name,
    DateOnly TargetDate,
    bool IsComplete);
