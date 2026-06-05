namespace Octopath_Traveler.Models.Passives.Observers;

public class StatSwapObserver : IPassiveObserver
{
    private readonly Traveler _traveler;

    public StatSwapObserver(Traveler traveler) => _traveler = traveler;

    public IReadOnlyList<string> Handle(IPassiveEvent passiveEvent)
    {
        if (passiveEvent is BattleStartEvent battleStart && battleStart.Traveler == _traveler)
            SwapAttackStats();
        return Array.Empty<string>();
    }

    private void SwapAttackStats()
    {
        int temp = _traveler.Stats.PhysicalAttack;
        _traveler.Stats.PhysicalAttack = _traveler.Stats.ElementalAttack;
        _traveler.Stats.ElementalAttack = temp;
    }
}
