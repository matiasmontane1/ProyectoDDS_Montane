namespace Octopath_Traveler.Models;

public readonly record struct DamageInput(int Attack, double Modifier, int Defense);
public readonly record struct DamageContext(bool IsWeakness, bool IsBreakingPoint);
