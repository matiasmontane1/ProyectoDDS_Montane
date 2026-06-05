namespace Octopath_Traveler.Models.Passives.Observers;

public class BoostStartObserver : IPassiveObserver
{
    private readonly Traveler _traveler;

    public BoostStartObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is BattleStartEvent battleStart && battleStart.Traveler == _traveler)
            _traveler.RecoverBp();
        return Array.Empty<string>();
    }
}
