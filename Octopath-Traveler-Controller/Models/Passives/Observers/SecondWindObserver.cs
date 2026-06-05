namespace Octopath_Traveler.Models.Passives.Observers;

public class SecondWindObserver : IPassiveObserver
{
    private const double RecoverPercent = 0.05;

    private readonly Traveler _traveler;

    public SecondWindObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is RoundEndEvent roundEnd && roundEnd.Traveler == _traveler)
            _traveler.RecoverSp((int)Math.Floor(_traveler.Stats.Sp * RecoverPercent));
        return Array.Empty<string>();
    }
}
