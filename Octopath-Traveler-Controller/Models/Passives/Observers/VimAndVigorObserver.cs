namespace Octopath_Traveler.Models.Passives.Observers;

public class VimAndVigorObserver : IPassiveObserver
{
    private const double HealPercent = 0.10;

    private readonly Traveler _traveler;

    public VimAndVigorObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is RoundEndEvent roundEnd && roundEnd.Traveler == _traveler)
            _traveler.Heal((int)Math.Floor(_traveler.Stats.Hp * HealPercent));
        return Array.Empty<string>();
    }
}
