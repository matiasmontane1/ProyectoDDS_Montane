using Octopath_Traveler.Models;
using Octopath_Traveler_View;

namespace Octopath_Traveler.Views;

public class CombatView
{
    private readonly View _view;
    private const string Separator = "----------------------------------------";

    public CombatView(View view) => _view = view;

    public void ShowPreTurnContext(IReadOnlyList<Traveler> playerTeam, IReadOnlyList<Beast> enemyTeam, IReadOnlyList<Unit> currentQueue, IReadOnlyList<Unit> nextRoundQueue)
    {
        ShowGameState(playerTeam, enemyTeam);
        ShowTurnOrder(currentQueue, "Turnos de la ronda");
        ShowTurnOrder(nextRoundQueue, "Turnos de la siguiente ronda");
    }

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

    private void ShowGameState(IReadOnlyList<Traveler> playerTeam, IReadOnlyList<Beast> enemyTeam)
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Equipo del jugador");
        for (int i = 0; i < playerTeam.Count; i++)
        {
            var traveler = playerTeam[i];
            _view.WriteLine($"{(char)('A' + i)}-{traveler.Name} - HP:{traveler.CurrentHp}/{traveler.Stats.Hp} SP:{traveler.CurrentSp}/{traveler.Stats.Sp} BP:{traveler.CurrentBp}");
        }
        _view.WriteLine("Equipo del enemigo");
        for (int i = 0; i < enemyTeam.Count; i++)
        {
            var beast = enemyTeam[i];
            _view.WriteLine($"{(char)('A' + i)}-{beast.Name} - HP:{beast.CurrentHp}/{beast.Stats.Hp} Shields:{beast.CurrentShields}");
        }
    }

    private void ShowTurnOrder(IReadOnlyList<Unit> queue, string title)
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

    public void ShowBeastDamage(string targetName, int damage, bool isPhysical)
    {
        string type = isPhysical ? "físico" : "elemental";
        _view.WriteLine($"{targetName} recibe {damage} de daño {type}");
    }

    public void ShowTypelessDamage(string targetName, int damage)
    {
        _view.WriteLine($"{targetName} recibe {damage} de daño");
    }

    public void ShowHeal(string targetName, int amount)
    {
        _view.WriteLine($"{targetName} recupera {amount} de vida");
    }

    public void ShowSpRecovery(string casterName, int amount)
    {
        _view.WriteLine($"{casterName} recupera {amount} SP");
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

    public void ShowStatusEffect(string targetName, string effectName, int rounds)
    {
        _view.WriteLine($"{targetName} tendrá {effectName} durante {rounds} rondas");
    }
}
