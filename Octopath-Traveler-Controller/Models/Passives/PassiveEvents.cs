namespace Octopath_Traveler.Models.Passives;

public record 
    BattleStartEvent(Traveler Traveler) : IPassiveEvent;

public class PatienceCheckEvent : IPassiveEvent
{
    public Traveler Traveler { get; }
    public bool PatienceGranted { get; set; }

    public PatienceCheckEvent(Traveler traveler) => Traveler = traveler;
}

public record RoundEndEvent(Traveler Traveler) : IPassiveEvent;

public class PreDamageEvent : IPassiveEvent
{
    public Unit Attacker { get; }
    public Unit Defender { get; }
    public int FinalDamage { get; set; }

    public PreDamageEvent(Unit attacker, Unit defender, int damage)
    {
        Attacker = attacker;
        Defender = defender;
        FinalDamage = damage;
    }
}

public record OnDamageAppliedEvent(Unit Defender, int PreDamageHp) : IPassiveEvent;

public class HealEvent : IPassiveEvent
{
    public Unit Target { get; }
    public int FinalAmount { get; set; }

    public HealEvent(Unit target, int amount)
    {
        Target = target;
        FinalAmount = amount;
    }
}

public class SkillUseEvent : IPassiveEvent
{
    public Traveler Caster { get; }
    public int FinalSpCost { get; set; }

    public SkillUseEvent(Traveler caster, int baseCost)
    {
        Caster = caster;
        FinalSpCost = baseCost;
    }
}

public record BasicAttackCompletedEvent(Traveler Attacker, int TotalDamage) : IPassiveEvent;

public class BuffGrantingEvent : IPassiveEvent
{
    public Traveler Caster { get; }
    public Traveler Target { get; }
    public StatusEffect Effect { get; }

    public BuffGrantingEvent(Traveler caster, Traveler target, StatusEffect effect)
    {
        Caster = caster;
        Target = target;
        Effect = effect;
    }
}

public record BuffAppliedEvent(Traveler Target, StatusEffect AppliedEffect) : IPassiveEvent;
