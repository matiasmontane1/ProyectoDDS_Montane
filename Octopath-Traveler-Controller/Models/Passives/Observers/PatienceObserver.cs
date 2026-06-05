namespace Octopath_Traveler.Models.Passives.Observers;

public class PatienceObserver : IPassiveObserver
{
    private readonly Traveler _traveler;

    public PatienceObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not PatienceCheckEvent checkEvent) return Array.Empty<string>();
        if (checkEvent.Traveler != _traveler) return Array.Empty<string>();
        if (!BothStatsAreEven()) return Array.Empty<string>();

        checkEvent.PatienceGranted = true;
        return Array.Empty<string>();
    }

    private bool BothStatsAreEven() =>
        _traveler.CurrentHp % 2 == 0 && _traveler.CurrentSp % 2 == 0;
}
