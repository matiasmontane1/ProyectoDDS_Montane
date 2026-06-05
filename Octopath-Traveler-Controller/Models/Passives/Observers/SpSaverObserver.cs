namespace Octopath_Traveler.Models.Passives.Observers;

public class SpSaverObserver : IPassiveObserver
{
    private const double CostDivisor = 2.0;

    private readonly Traveler _traveler;

    public SpSaverObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is not SkillUseEvent skillEvent) return Array.Empty<string>();
        if (skillEvent.Caster != _traveler) return Array.Empty<string>();

        skillEvent.FinalSpCost = (int)Math.Floor(skillEvent.FinalSpCost / CostDivisor);
        return Array.Empty<string>();
    }
}
