namespace Octopath_Traveler.Models.Passives.Observers;

public class InspirationObserver : IPassiveObserver
{
    private const double SpRecoveryPercent = 0.01;

    private readonly Traveler _traveler;

    public InspirationObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not BasicAttackCompletedEvent attackEvent) return Array.Empty<string>();
        if (attackEvent.Attacker != _traveler) return Array.Empty<string>();

        int recoveredSp = (int)Math.Floor(attackEvent.TotalDamage * SpRecoveryPercent);
        _traveler.RecoverSp(recoveredSp);
        return new[] { $"{_traveler.Name} recupera {recoveredSp} SP" };
    }
}
