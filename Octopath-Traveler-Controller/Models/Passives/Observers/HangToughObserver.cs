namespace Octopath_Traveler.Models.Passives.Observers;

public class HangToughObserver : IPassiveObserver
{
    private const double ActivationThresholdPercent = 0.10;

    private readonly Traveler _traveler;

    public HangToughObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not PreDamageEvent damageEvent) return Array.Empty<string>();
        if (damageEvent.Defender != _traveler) return Array.Empty<string>();
        if (!WouldBeLethal(damageEvent.FinalDamage)) return Array.Empty<string>();
        if (!IsAboveActivationThreshold()) return Array.Empty<string>();

        damageEvent.FinalDamage = _traveler.CurrentHp - 1;
        return Array.Empty<string>();
    }

    private bool WouldBeLethal(int damage) => damage >= _traveler.CurrentHp;

    private bool IsAboveActivationThreshold() =>
        _traveler.CurrentHp > (int)Math.Floor(_traveler.Stats.Hp * ActivationThresholdPercent);
}
