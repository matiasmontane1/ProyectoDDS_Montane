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

        return CompileQueue(unit => unit.EffectiveSpeed, recoveryFirst, defenderFirst, spearheadFirst, normal, legholded);
    }

    public List<Unit> GenerateNextRoundPreview(List<Unit> currentRemainingQueue)
    {
        var recoveringBeasts = _enemyTeam.Where(beast => !beast.IsDead && beast.BreakingPointRoundsRemaining == 1);
        var recoveryFirst = recoveringBeasts.Select(beast => ((Unit)beast, false, _enemyTeam.IndexOf(beast)));

        var defenderFirst = GetDefenderFirst(excludeActed: true, nextRound: true, currentRemainingQueue);
        var spearheadFirst = GetSpearheadFirst(excludeActed: true, nextRound: true, currentRemainingQueue);
        var normal = GetNormalQueue(excludeActed: true, nextRound: true, currentRemainingQueue);
        var legholded = GetDesprioritizedQueue(nextRound: true);

        return CompileQueue(unit => unit.EffectiveSpeedNextRound, recoveryFirst, defenderFirst, spearheadFirst, normal, legholded);
    }

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetRecoveryFirst()
    {
        var justRecoveredBeasts = _enemyTeam.Where(beast => !beast.IsDead && beast.JustRecoveredFromBreakingPoint);
        return justRecoveredBeasts.Select(beast => ((Unit)beast, false, _enemyTeam.IndexOf(beast)));
    }

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetDefenderFirst(bool excludeActed, bool nextRound, List<Unit>? queue = null)
    {
        var eligibleDefenders = _playerTeam.Where(traveler => IsEligibleDefender(traveler, nextRound, excludeActed, queue));
        return eligibleDefenders.Select(traveler => ((Unit)traveler, true, _playerTeam.IndexOf(traveler)));
    }

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetSpearheadFirst(bool excludeActed, bool nextRound, List<Unit>? queue = null)
    {
        var eligibleSpearheads = _playerTeam.Where(traveler => IsEligibleSpearhead(traveler, nextRound, excludeActed, queue));
        return eligibleSpearheads.Select(traveler => ((Unit)traveler, true, _playerTeam.IndexOf(traveler)));
    }

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetNormalQueue(bool excludeActed, bool nextRound, List<Unit>? queue = null)
    {
        var normalTravelers = _playerTeam.Where(traveler => IsNormalQueueTraveler(traveler, excludeActed, queue));
        var travelerEntries = normalTravelers.Select(traveler => ((Unit)traveler, true, _playerTeam.IndexOf(traveler)));

        var normalBeasts = _enemyTeam.Where(beast => IsNormalQueueBeast(beast, nextRound));
        var beastEntries = normalBeasts.Select(beast => ((Unit)beast, false, _enemyTeam.IndexOf(beast)));

        return travelerEntries.Concat(beastEntries);
    }

    private IEnumerable<(Unit Unit, bool IsTraveler, int Index)> GetDesprioritizedQueue(bool nextRound)
    {
        var desprioritizedBeasts = _enemyTeam.Where(beast => IsDesprioritizedQueueBeast(beast, nextRound));
        return desprioritizedBeasts.Select(beast => ((Unit)beast, false, _enemyTeam.IndexOf(beast)));
    }

    private static bool IsEligibleDefender(Traveler traveler, bool nextRound, bool excludeActed, List<Unit>? queue)
    {
        bool isCurrentlyDefending = nextRound ? traveler.IsDefending : traveler.DefendedLastRound;
        bool hasNotActed = !excludeActed || queue == null || !queue.Contains(traveler);
        return isCurrentlyDefending && !traveler.IsDead && hasNotActed;
    }

    private static bool IsEligibleSpearhead(Traveler traveler, bool nextRound, bool excludeActed, List<Unit>? queue)
    {
        bool isCurrentlyDefending = nextRound ? traveler.IsDefending : traveler.DefendedLastRound;
        bool hasNotActed = !excludeActed || queue == null || !queue.Contains(traveler);
        return traveler.HasPriorityNextRound && !isCurrentlyDefending && !traveler.IsDead && hasNotActed;
    }

    private static bool IsNormalQueueTraveler(Traveler traveler, bool excludeActed, List<Unit>? queue)
    {
        bool hasNotActed = !excludeActed || queue == null || !queue.Contains(traveler);
        return !(traveler.HasPriorityNextRound && hasNotActed) && !traveler.IsDead;
    }

    private static bool IsNormalQueueBeast(Beast beast, bool nextRound)
    {
        bool isBreakingPointFree = nextRound ? beast.BreakingPointRoundsRemaining == 0 : (!beast.IsInBreakingPoint && !beast.JustRecoveredFromBreakingPoint);
        bool isNotDesprioritized = nextRound ? !(beast.IsDesprioritized && beast.DesprioritizationRoundsRemaining > 1) : !beast.IsDesprioritized;
        return !beast.IsDead && isBreakingPointFree && isNotDesprioritized;
    }

    private static bool IsDesprioritizedQueueBeast(Beast beast, bool nextRound)
    {
        bool isBreakingPointFree = nextRound ? beast.BreakingPointRoundsRemaining == 0 : (!beast.IsInBreakingPoint && !beast.JustRecoveredFromBreakingPoint);
        bool isDesprioritized = nextRound ? beast.IsDesprioritized && beast.DesprioritizationRoundsRemaining > 1 : beast.IsDesprioritized;
        return !beast.IsDead && isBreakingPointFree && isDesprioritized;
    }

    private List<Unit> CompileQueue(Func<Unit, int> getSpeed, params IEnumerable<(Unit Unit, bool IsTraveler, int Index)>[] segments)
    {
        var result = new List<Unit>();

        foreach (var segment in segments)
        {
            var orderedSegment = segment
                .OrderByDescending(entry => getSpeed(entry.Unit))
                .ThenByDescending(entry => entry.IsTraveler)
                .ThenBy(entry => entry.Index);
            var sortedUnits = orderedSegment.Select(entry => entry.Unit);
            result.AddRange(sortedUnits);
        }

        return result;
    }
}
