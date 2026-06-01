using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers.SkillHandlers;

public class BasicAttackHandler
{
    private const double BasicAttackModifier = 1.3;

    private readonly CombatView _view;
    private readonly CombatMenuView _menuView;
    private readonly List<Beast> _enemyTeam;

    public BasicAttackHandler(CombatView view, CombatMenuView menuView, List<Beast> enemyTeam)
    {
        _view = view;
        _menuView = menuView;
        _enemyTeam = enemyTeam;
    }

    public bool Execute(Traveler traveler)
    {
        string? weapon = _menuView.PromptWeaponSelection(traveler.Weapons);
        if (weapon == null) return false;

        var aliveEnemies = _enemyTeam.Where(enemy => !enemy.IsDead).ToList();
        Beast? target = _menuView.PromptBeastTargetSelection(traveler.Name, aliveEnemies);
        if (target == null) return false;

        int bpUsed = _menuView.PromptBpUsage(traveler.Name, traveler.CurrentBp);
        traveler.SpendBp(bpUsed);

        ExecuteMultiHitAttack(traveler, target, weapon, 1 + bpUsed);
        return true;
    }

    private void ExecuteMultiHitAttack(Traveler traveler, Beast target, string weapon, int hitCount)
    {
        _view.ShowTravelerAttacks(traveler.Name);

        for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
            ApplySingleHit(traveler, target, weapon);

        _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ApplySingleHit(Traveler traveler, Beast target, string weapon)
    {
        bool isWeakness = target.Weaknesses.Contains(weapon);
        var input = new DamageInput(traveler.Stats.PhysicalAttack, BasicAttackModifier, target.Stats.PhysicalDefense);
        var context = new DamageContext(isWeakness, target.IsInBreakingPoint, traveler.PhysicalAttackMultiplier, target.PhysicalDefenseMultiplier);
        int damage = DamageCalculator.CalculatePhysicalDamage(input, context);

        target.TakeDamage(damage);
        _view.ShowDamageWithType(target.Name, damage, weapon, isWeakness);

        if (isWeakness && damage > 0)
            ProcessShieldDamage(target);
    }

    private void ProcessShieldDamage(Beast target) =>
        ShieldDamageProcessor.ProcessWeaknessHit(target, _view);
}
