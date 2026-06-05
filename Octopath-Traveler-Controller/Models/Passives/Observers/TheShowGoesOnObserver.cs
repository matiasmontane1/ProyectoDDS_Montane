namespace Octopath_Traveler.Models.Passives.Observers;

public class TheShowGoesOnObserver : IPassiveObserver
{
    private const int BonusRounds = 1;

    private readonly Traveler _traveler;

    public TheShowGoesOnObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not BuffGrantingEvent buffEvent) return Array.Empty<string>();
        if (buffEvent.Caster != _traveler) return Array.Empty<string>();

        buffEvent.Effect.RemainingRounds += BonusRounds;
        return Array.Empty<string>();
    }
}
