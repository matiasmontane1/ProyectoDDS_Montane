namespace Octopath_Traveler.Models.Passives.Observers;

public class EncoreObserver : IPassiveObserver
{
    private readonly Traveler _traveler;

    public EncoreObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not OnDamageAppliedEvent damageEvent) return Array.Empty<string>();
        if (damageEvent.Defender != _traveler) return Array.Empty<string>();
        if (!_traveler.IsDead || _traveler.EncoreUsed) return Array.Empty<string>();

        _traveler.UseEncore();
        return new[] { $"{_traveler.Name} revive" };
    }
}
