namespace Octopath_Traveler.Models.Passives.Observers;

public class DivineAuraObserver : IPassiveObserver
{
    private readonly Traveler _traveler;

    public DivineAuraObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not PreDamageEvent damageEvent) return Array.Empty<string>();
        if (damageEvent.Defender != _traveler) return Array.Empty<string>();
        if (!BothHpAreEven(damageEvent)) return Array.Empty<string>();

        damageEvent.FinalDamage = 0;
        return Array.Empty<string>();
    }

    private static bool BothHpAreEven(PreDamageEvent damageEvent) =>
        damageEvent.Defender.CurrentHp % 2 == 0 && damageEvent.Attacker.CurrentHp % 2 == 0;
}
