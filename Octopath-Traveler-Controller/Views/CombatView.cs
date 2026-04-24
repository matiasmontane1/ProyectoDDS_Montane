using Octopath_Traveler.Models;
using Octopath_Traveler_View;

namespace Octopath_Traveler.Views;

public class CombatView
{
    private readonly View _view;
    private const string Separator = "----------------------------------------";

    public CombatView(View view) => _view = view;

    public void ShowRoundStart(int round)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"INICIA RONDA {round}");
    }

    public void ShowPlayerTeamWins()
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Gana equipo del jugador");
    }

    public void ShowEnemyTeamWins()
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Gana equipo del enemigo");
    }

    public void ShowFleeResult()
    {
        _view.WriteLine(Separator);
        _view.WriteLine("El equipo de viajeros ha huido!");
        _view.WriteLine(Separator);
        _view.WriteLine("Gana equipo del enemigo");
    }

    public void ShowGameState(IReadOnlyList<Traveler> playerTeam, IReadOnlyList<Beast> enemyTeam)
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Equipo del jugador");
        for (int i = 0; i < playerTeam.Count; i++)
        {
            var t = playerTeam[i];
            _view.WriteLine($"{(char)('A' + i)}-{t.Name} - HP:{t.CurrentHp}/{t.Stats.Hp} SP:{t.CurrentSp}/{t.Stats.Sp} BP:{t.CurrentBp}");
        }
        _view.WriteLine("Equipo del enemigo");
        for (int i = 0; i < enemyTeam.Count; i++)
        {
            var b = enemyTeam[i];
            _view.WriteLine($"{(char)('A' + i)}-{b.Name} - HP:{b.CurrentHp}/{b.Stats.Hp} Shields:{b.CurrentShields}");
        }
    }

    public void ShowTurnOrder(IReadOnlyList<Unit> queue, string title)
    {
        _view.WriteLine(Separator);
        _view.WriteLine(title);
        for (int i = 0; i < queue.Count; i++)
            _view.WriteLine($"{i + 1}.{queue[i].Name}");
    }
    
    public void ShowTravelerAttacks(string travelerName)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"{travelerName} ataca");
    }

    public void ShowUnitUsesSkill(string unitName, string skillName)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"{unitName} usa {skillName}");
    }

    public void ShowDamageWithType(string targetName, int damage, string type, bool isWeakness)
    {
        string suffix = isWeakness ? " con debilidad" : "";
        _view.WriteLine($"{targetName} recibe {damage} de daño de tipo {type}{suffix}");
    }

    public void ShowTypelessDamage(string targetName, int damage)
    {
        _view.WriteLine($"{targetName} recibe {damage} de daño");
    }

    public void ShowBeastDamage(string targetName, int damage, bool isPhysical)
    {
        string type = isPhysical ? "físico" : "elemental";
        _view.WriteLine($"{targetName} recibe {damage} de daño {type}");
    }

    public void ShowHeal(string targetName, int amount)
    {
        _view.WriteLine($"{targetName} recupera {amount} de vida");
    }

    public void ShowRevive(string targetName)
    {
        _view.WriteLine($"{targetName} revive");
    }

    public void ShowFinalHp(string name, int hp)
    {
        _view.WriteLine($"{name} termina con HP:{hp}");
    }

    public void ShowBreakingPoint(string beastName)
    {
        _view.WriteLine($"{beastName} entra en Breaking Point");
    }

    public void ShowLeghold(string targetName, int rounds)
    {
        _view.WriteLine($"{targetName} tendrá menor prioridad de turno durante {rounds} rondas");
    }

    public void ShowDefending(string travelerName)
    {
        _view.WriteLine($"{travelerName} se defiende");
    }


    public void ShowTravelerActionMenu(string travelerName)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Turno de {travelerName}");
        _view.WriteLine("1: Ataque básico");
        _view.WriteLine("2: Usar habilidad");
        _view.WriteLine("3: Defender");
        _view.WriteLine("4: Huir");
    }

    public string ReadLine() => _view.ReadLine();

    public void PromptBpUsageIfAvailable(int currentBp)
    {
        if (currentBp < 1) return;
        _view.WriteLine(Separator);
        _view.WriteLine("Seleccione cuantos BP utilizar");
        _view.ReadLine();
    }

    public string? PromptWeaponSelection(IReadOnlyList<string> weapons)
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Seleccione un arma");
        for (int i = 0; i < weapons.Count; i++)
            _view.WriteLine($"{i + 1}: {weapons[i]}");
        _view.WriteLine($"{weapons.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice > 0 && choice <= weapons.Count)
            return weapons[choice - 1];
        return null;
    }

    public Beast? PromptBeastTargetSelection(string travelerName, IReadOnlyList<Beast> targets)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Seleccione un objetivo para {travelerName}");
        for (int i = 0; i < targets.Count; i++)
        {
            var e = targets[i];
            _view.WriteLine($"{i + 1}: {e.Name} - HP:{e.CurrentHp}/{e.Stats.Hp} Shields:{e.CurrentShields}");
        }
        _view.WriteLine($"{targets.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice > 0 && choice <= targets.Count)
            return targets[choice - 1];
        return null;
    }

    public Traveler? PromptAllyTargetSelection(string travelerName, IReadOnlyList<Traveler> targets)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Seleccione un objetivo para {travelerName}");
        for (int i = 0; i < targets.Count; i++)
        {
            var t = targets[i];
            _view.WriteLine($"{i + 1}: {t.Name} - HP:{t.CurrentHp}/{t.Stats.Hp} SP:{t.CurrentSp}/{t.Stats.Sp} BP:{t.CurrentBp}");
        }
        _view.WriteLine($"{targets.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice > 0 && choice <= targets.Count)
            return targets[choice - 1];
        return null;
    }

    public string? PromptSkillSelection(string travelerName, IReadOnlyList<string> availableSkills)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Seleccione una habilidad para {travelerName}");
        for (int i = 0; i < availableSkills.Count; i++)
            _view.WriteLine($"{i + 1}: {availableSkills[i]}");
        _view.WriteLine($"{availableSkills.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice >= 1 && choice <= availableSkills.Count)
            return availableSkills[choice - 1];
        return null;
    }
}
