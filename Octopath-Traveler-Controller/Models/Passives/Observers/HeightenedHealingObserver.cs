namespace Octopath_Traveler.Models.Passives.Observers;

public class HeightenedHealingObserver : IPassiveObserver
{
    private const double HealMultiplier = 1.3;

    private readonly Traveler _traveler;

    public HeightenedHealingObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not HealEvent healEvent) return Array.Empty<string>();
        if (healEvent.Target != _traveler) return Array.Empty<string>();

        healEvent.FinalAmount = (int)Math.Floor(healEvent.FinalAmount * HealMultiplier);
        return Array.Empty<string>();
    }
}
