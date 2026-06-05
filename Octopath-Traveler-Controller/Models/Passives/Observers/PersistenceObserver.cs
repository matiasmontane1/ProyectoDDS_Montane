namespace Octopath_Traveler.Models.Passives.Observers;

public class PersistenceObserver : IPassiveObserver
{
    private const int BonusRounds = 1;

    private readonly Traveler _traveler;

    public PersistenceObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not BuffAppliedEvent buffEvent) return Array.Empty<string>();
        if (buffEvent.Target != _traveler) return Array.Empty<string>();

        buffEvent.AppliedEffect.RemainingRounds += BonusRounds;
        return Array.Empty<string>();
    }
}
