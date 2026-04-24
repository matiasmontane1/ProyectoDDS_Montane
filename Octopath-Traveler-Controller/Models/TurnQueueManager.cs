namespace Octopath_Traveler.Models;

public class TurnQueueManager
{
    private readonly List<Traveler> _playerTeam;
    private readonly List<Beast> _enemyTeam;

    public TurnQueueManager(List<Traveler> playerTeam, List<Beast> enemyTeam)
    {
        _playerTeam = playerTeam;
        _enemyTeam = enemyTeam;
    }

    public List<Unit> GenerateCurrentRoundQueue()
    {
        var recoveryFirst = GetRecoveryFirst();
        var defenderFirst = GetDefenderFirst(excludeActed: false, nextRound: false);
        var spearheadFirst = GetSpearheadFirst(excludeActed: false, nextRound: false);
        var normal = GetNormalQueue(excludeActed: false, nextRound: false);
        var legholded = GetDesprioritizedQueue(nextRound: false);

        return CompileQueue(recoveryFirst, defenderFirst, spearheadFirst, normal, legholded);
    }

    public List<Unit> GenerateNextRoundPreview(List<Unit> currentRemainingQueue)
    {
        var recoveryFirst = _enemyTeam
            .Where(b => !b.IsDead && b.BreakingPointRoundsRemaining == 1)
            .Select(b => ((Unit)b, false, _enemyTeam.IndexOf(b)));

        var defenderFirst = GetDefenderFirst(excludeActed: true, nextRound: true, currentRemainingQueue);
        var spearheadFirst = GetSpearheadFirst(excludeActed: true, nextRound: true, currentRemainingQueue);
        var normal = GetNormalQueue(excludeActed: true, nextRound: true, currentRemainingQueue);
        var legholded = GetDesprioritizedQueue(nextRound: true);

        return CompileQueue(recoveryFirst, defenderFirst, spearheadFirst, normal, legholded);
    }

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetRecoveryFirst() => _enemyTeam
        .Where(b => !b.IsDead && b.JustRecoveredFromBreakingPoint)
        .Select(b => ((Unit)b, false, _enemyTeam.IndexOf(b)));

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetDefenderFirst(bool excludeActed, bool nextRound, List<Unit>? queue = null) => _playerTeam
        .Where(t => (nextRound ? t.IsDefending : t.DefendedLastRound) && !t.IsDead && (!excludeActed || queue == null || !queue.Contains(t)))
        .Select(t => ((Unit)t, true, _playerTeam.IndexOf(t)));

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetSpearheadFirst(bool excludeActed, bool nextRound, List<Unit>? queue = null) => _playerTeam
        .Where(t => t.HasPriorityNextRound && !(nextRound ? t.IsDefending : t.DefendedLastRound) && !t.IsDead && (!excludeActed || queue == null || !queue.Contains(t)))
        .Select(t => ((Unit)t, true, _playerTeam.IndexOf(t)));

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetNormalQueue(bool excludeActed, bool nextRound, List<Unit>? queue = null)
    {
        var travelers = _playerTeam
            .Where(t => !(t.HasPriorityNextRound && (!excludeActed || queue == null || !queue.Contains(t))) && !t.IsDead)
            .Select(t => ((Unit)t, true, _playerTeam.IndexOf(t)));

        var beasts = _enemyTeam
            .Where(b => !b.IsDead && 
                        (nextRound ? b.BreakingPointRoundsRemaining == 0 : (!b.IsInBreakingPoint && !b.JustRecoveredFromBreakingPoint)) &&
                        (nextRound ? !(b.IsDesprioritized && b.DesprioritizationRoundsRemaining > 1) : !b.IsDesprioritized))
            .Select(b => ((Unit)b, false, _enemyTeam.IndexOf(b)));

        return travelers.Concat(beasts);
    }

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetDesprioritizedQueue(bool nextRound) => _enemyTeam
        .Where(b => !b.IsDead && 
                    (nextRound ? b.BreakingPointRoundsRemaining == 0 : (!b.IsInBreakingPoint && !b.JustRecoveredFromBreakingPoint)) && 
                    (nextRound ? b.IsDesprioritized && b.DesprioritizationRoundsRemaining > 1 : b.IsDesprioritized))
        .Select(b => ((Unit)b, false, _enemyTeam.IndexOf(b)));

    private List<Unit> CompileQueue(params IEnumerable<(Unit Unit, bool IsTraveler, int Index)>[] segments)
    {
        var result = new List<Unit>();
        
        foreach (var segment in segments)
        {
            var sortedSegment = segment
                .OrderByDescending(x => x.Unit.Stats.Speed)
                .ThenByDescending(x => x.IsTraveler)
                .ThenBy(x => x.Index)
                .Select(x => x.Unit);
            
            result.AddRange(sortedSegment);
        }
        
        return result;
    }
}