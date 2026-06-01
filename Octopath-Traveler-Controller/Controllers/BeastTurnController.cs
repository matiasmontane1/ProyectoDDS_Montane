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
        var beastSkill = _beastSkills.FirstOrDefault(skill => skill.Name == beast.Skill);
        if (beastSkill == null) return;

        _view.ShowUnitUsesSkill(beast.Name, beast.Skill);

        if (beastSkill.IsVortalClaw)  { ExecuteVortalClaw(); return; }
        if (beastSkill.IsNonDamaging) return;
        if (beastSkill.IsAoe)         ExecuteAoeSkill(beast, beastSkill);
        else                          ExecuteSingleTargetSkill(beast, beastSkill);
    }

    private void ExecuteVortalClaw()
    {
        var targets = _playerTeam.Where(traveler => !traveler.IsDead).ToList();
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

    private void ExecuteAoeSkill(Beast beast, BeastSkill beastSkill)
    {
        var targets = _playerTeam.Where(traveler => !traveler.IsDead).ToList();
        foreach (var target in targets)
            ApplyBeastAttack(beast, target, beastSkill);
        foreach (var target in targets)
            _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ExecuteSingleTargetSkill(Beast beast, BeastSkill beastSkill)
    {
        var target = SelectTarget(beastSkill);
        if (target == null) return;
        ApplyBeastAttack(beast, target, beastSkill);
        _view.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ApplyBeastAttack(Beast beast, Traveler target, BeastSkill beastSkill)
    {
        double attackMultiplier = beastSkill.IsPhysical ? beast.PhysicalAttackMultiplier : beast.ElementalAttackMultiplier;
        double defenseMultiplier = beastSkill.IsPhysical ? target.PhysicalDefenseMultiplier : target.ElementalDefenseMultiplier;
        var context = new DamageContext(IsWeakness: false, IsBreakingPoint: false, AttackMultiplier: attackMultiplier, DefenseMultiplier: defenseMultiplier);
        int damage = beastSkill.IsPhysical
            ? DamageCalculator.CalculatePhysicalDamage(new DamageInput(beast.Stats.PhysicalAttack, beastSkill.Modifier, target.Stats.PhysicalDefense), context)
            : DamageCalculator.CalculateElementalDamage(new DamageInput(beast.Stats.ElementalAttack, beastSkill.Modifier, target.Stats.ElementalDefense), context);

        if (target.IsDefending)
        {
            _view.ShowDefending(target.Name);
            damage = (int)Math.Floor(damage / 2.0);
        }

        target.TakeDamage(damage);
        _view.ShowBeastDamage(target.Name, damage, beastSkill.IsPhysical);
    }

    private Traveler? SelectTarget(BeastSkill beastSkill)
    {
        var aliveTravelers = _playerTeam.Where(traveler => !traveler.IsDead).ToList();
        if (aliveTravelers.Count == 0) return null;

        return beastSkill.TargetCriteria switch
        {
            "MaxElemAtk" => aliveTravelers.OrderByDescending(traveler => traveler.Stats.ElementalAttack).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MinPhysDef" => aliveTravelers.OrderBy(traveler => traveler.Stats.PhysicalDefense).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MaxSpeed"   => aliveTravelers.OrderByDescending(traveler => traveler.Stats.Speed).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MinElemDef" => aliveTravelers.OrderBy(traveler => traveler.Stats.ElementalDefense).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MaxPhysDef" => aliveTravelers.OrderByDescending(traveler => traveler.Stats.PhysicalDefense).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MaxPhysAtk" => aliveTravelers.OrderByDescending(traveler => traveler.Stats.PhysicalAttack).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            "MinSpeed"   => aliveTravelers.OrderBy(traveler => traveler.Stats.Speed).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
            _            => aliveTravelers.OrderByDescending(traveler => traveler.CurrentHp).ThenBy(traveler => _playerTeam.IndexOf(traveler)).First(),
        };
    }
}
