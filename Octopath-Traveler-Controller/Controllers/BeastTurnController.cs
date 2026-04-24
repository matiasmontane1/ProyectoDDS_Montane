using Octopath_Traveler.Models;
using Octopath_Traveler.Views;

namespace Octopath_Traveler.Controllers;

public class BeastTurnController
{
    private readonly CombatView _view;
    private readonly List<Traveler> _playerTeam;
    private readonly List<BeastSkill> _beastSkills;

    public BeastTurnController(CombatView view, List<Traveler> playerTeam, List<BeastSkill> beastSkills)
    {
        _view = view;
        _playerTeam = playerTeam;
        _beastSkills = beastSkills;
    }

    public void HandleTurn(Beast beast)
    {
        beast.ClearRecovery();
        var skill = _beastSkills.FirstOrDefault(s => s.Name == beast.Skill);
        if (skill == null) return;

        _view.ShowUnitUsesSkill(beast.Name, beast.Skill);

        if (skill.IsVortalClaw)  { ExecuteVortalClaw(); return; }
        if (skill.IsNonDamaging) return;
        if (skill.IsAoe)         ExecuteAoeSkill(beast, skill);
        else                     ExecuteSingleTargetSkill(beast, skill);
    }

    private void ExecuteVortalClaw()
    {
        var targets = _playerTeam.Where(t => !t.IsDead).ToList();
        foreach (var target in targets)
        {
            int newHp = (int)Math.Floor(target.CurrentHp / 2.0);
            int damage = target.CurrentHp - newHp;
            target.TakeDamage(damage);
            _view.ShowTypelessDamage(target.Name, damage);
        }
        foreach (var target in targets)
            _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ExecuteAoeSkill(Beast beast, BeastSkill skill)
    {
        var targets = _playerTeam.Where(t => !t.IsDead).ToList();
        foreach (var target in targets)
            ApplyBeastAttack(beast, target, skill);
        foreach (var target in targets)
            _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ExecuteSingleTargetSkill(Beast beast, BeastSkill skill)
    {
        var target = SelectTarget(skill);
        if (target == null) return;
        ApplyBeastAttack(beast, target, skill);
        _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ApplyBeastAttack(Beast beast, Traveler target, BeastSkill skill)
    {
        int damage = skill.IsPhysical
            ? DamageCalculator.CalculatePhysicalDamage(beast.Stats.PhysicalAttack, skill.Modifier, target.Stats.PhysicalDefense, false, false)
            : DamageCalculator.CalculateElementalDamage(beast.Stats.ElementalAttack, skill.Modifier, target.Stats.ElementalDefense, false, false);

        if (target.IsDefending)
        {
            _view.ShowDefending(target.Name);
            damage = (int)Math.Floor(damage / 2.0);
        }

        target.TakeDamage(damage);
        _view.ShowBeastDamage(target.Name, damage, skill.IsPhysical);
    }

    private Traveler? SelectTarget(BeastSkill skill)
    {
        var alive = _playerTeam.Where(t => !t.IsDead).ToList();
        if (alive.Count == 0) return null;

        return skill.TargetCriteria switch
        {
            "MaxElemAtk" => alive.OrderByDescending(t => t.Stats.ElementalAttack).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MinPhysDef" => alive.OrderBy(t => t.Stats.PhysicalDefense).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MaxSpeed"   => alive.OrderByDescending(t => t.Stats.Speed).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MinElemDef" => alive.OrderBy(t => t.Stats.ElementalDefense).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MaxPhysDef" => alive.OrderByDescending(t => t.Stats.PhysicalDefense).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MaxPhysAtk" => alive.OrderByDescending(t => t.Stats.PhysicalAttack).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MinSpeed"   => alive.OrderBy(t => t.Stats.Speed).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            _            => alive.OrderByDescending(t => t.CurrentHp).ThenBy(t => _playerTeam.IndexOf(t)).First(),
        };
    }
}
