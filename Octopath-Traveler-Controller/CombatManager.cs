using Octopath_Traveler_View;
using Octopath_Traveler.Models;

namespace Octopath_Traveler;

public class CombatManager
{
    private readonly View _view;
    private readonly List<Traveler> _playerTeam;
    private readonly List<Beast> _enemyTeam;
    private int _currentRound;
    
    private const string Separator = "----------------------------------------";

    public CombatManager(View view, List<Traveler> playerTeam, List<Beast> enemyTeam)
    {
        _view = view;
        _playerTeam = playerTeam;
        _enemyTeam = enemyTeam;
        _currentRound = 1;

        foreach (var traveler in _playerTeam) traveler.InitializeState();
        foreach (var beast in _enemyTeam) beast.InitializeState();
    }

    public void StartCombat()
    {
        while (true)
        {
            if (CheckWinCondition(out string winnerMessage))
            {
                PrintMessageWithSeparator(winnerMessage);
                return; 
            }
            
            _view.WriteLine(Separator);
            _view.WriteLine($"INICIA RONDA {_currentRound}");
            
            var turnQueue = GenerateTurnQueue().Where(u => !u.IsDead).ToList();
            
            while (turnQueue.Count > 0)
            {
                if (CheckWinCondition(out string innerWinnerMessage))
                {
                    PrintMessageWithSeparator(innerWinnerMessage);
                    return; 
                }

                bool battleEnded = ProcessSingleTurn(turnQueue);
                if (battleEnded) return;
            }
            
            RecoverTravelersBP();
            _currentRound++;
        }
    }

    private bool ProcessSingleTurn(List<Unit> turnQueue)
    {
        var currentUnit = turnQueue[0];

        if (currentUnit.IsDead) 
        {
            turnQueue.RemoveAt(0);
            return false;
        }

        PrintGameState();
        PrintTurnOrder(turnQueue, "Turnos de la ronda");
        
        var nextRoundQueue = GenerateTurnQueue().Where(u => !u.IsDead).ToList();
        PrintTurnOrder(nextRoundQueue, "Turnos de la siguiente ronda");

        if (currentUnit is Traveler traveler)
        {
            bool fled = HandleTravelerTurn(traveler);
            if (fled) return true; // Fin del combate
        }
        else if (currentUnit is Beast beast)
        {
            HandleBeastTurn(beast);
        }
        
        turnQueue.RemoveAt(0);
        turnQueue.RemoveAll(u => u.IsDead);
        
        return false; // El combate sigue
    }

    private void RecoverTravelersBP()
    {
        foreach (var traveler in _playerTeam.Where(t => !t.IsDead))
        {
            traveler.RecoverBP();
        }
    }

    private bool HandleTravelerTurn(Traveler traveler)
    {
        while (true)
        {
            _view.WriteLine(Separator);
            _view.WriteLine($"Turno de {traveler.Name}");
            _view.WriteLine("1: Ataque básico");
            _view.WriteLine("2: Usar habilidad");
            _view.WriteLine("3: Defender");
            _view.WriteLine("4: Huir");

            string input = _view.ReadLine();

            if (input == "1" && ExecuteBasicAttack(traveler)) return false;
            if (input == "2") HandleSkillMenu(traveler);
            if (input == "3") return false; 
            if (input == "4")
            {
                PrintMessageWithSeparator("El equipo de viajeros ha huido!");
                _view.WriteLine(Separator);
                _view.WriteLine("Gana equipo del enemigo");
                return true;
            }
        }
    }

    private bool ExecuteBasicAttack(Traveler traveler)
    {
        string selectedWeapon = PromptWeaponSelection(traveler);
        if (selectedWeapon == null) return false; // El jugador canceló

        var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
        Beast target = PromptTargetSelection(traveler, aliveEnemies);
        if (target == null) return false; // El jugador canceló

        PromptBPUsage(traveler);

        ResolveBasicAttack(traveler, target, selectedWeapon);

        return true;
    }
    
    private string PromptWeaponSelection(Traveler traveler)
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Seleccione un arma");
        
        for (int i = 0; i < traveler.Weapons.Count; i++)
        {
            _view.WriteLine($"{i + 1}: {traveler.Weapons[i]}");
        }
        _view.WriteLine($"{traveler.Weapons.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice > 0 && choice <= traveler.Weapons.Count)
        {
            return traveler.Weapons[choice - 1];
        }
        
        return null; 
    }

    private Beast PromptTargetSelection(Traveler traveler, List<Beast> aliveEnemies)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Seleccione un objetivo para {traveler.Name}");
        
        for (int i = 0; i < aliveEnemies.Count; i++)
        {
            var e = aliveEnemies[i];
            _view.WriteLine($"{i + 1}: {e.Name} - HP:{e.CurrentHP}/{e.Stats.HP} Shields:{e.CurrentShields}");
        }
        _view.WriteLine($"{aliveEnemies.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice > 0 && choice <= aliveEnemies.Count)
        {
            return aliveEnemies[choice - 1];
        }
        
        return null;
    }

    private void PromptBPUsage(Traveler traveler)
    {
        if (traveler.CurrentBP >= 1)
        {
            _view.WriteLine(Separator);
            _view.WriteLine("Seleccione cuantos BP utilizar");
            _view.ReadLine();
        }
    }

    private void ResolveBasicAttack(Traveler traveler, Beast target, string weapon)
    {
        int damage = CalculatePhysicalDamage(traveler.Stats.PhysAtk, target.Stats.PhysDef);
        target.TakeDamage(damage);

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} ataca");
        _view.WriteLine($"{target.Name} recibe {damage} de daño de tipo {weapon}");
        _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
    }

    private void HandleSkillMenu(Traveler traveler)
    {
        _view.WriteLine(Separator); 
        _view.WriteLine($"Seleccione una habilidad para {traveler.Name}");
        
        int count = 0;
        if (traveler.Skills != null)
        {
            for (int i = 0; i < traveler.Skills.Count; i++)
            {
                _view.WriteLine($"{i + 1}: {traveler.Skills[i]}");
                count++;
            }
        }

        _view.WriteLine($"{count + 1}: Cancelar");
        _view.ReadLine();
    }

    private void HandleBeastTurn(Beast beast)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"{beast.Name} usa {beast.Skill}");
        var target = _playerTeam.Where(t => !t.IsDead).OrderByDescending(t => t.CurrentHP).FirstOrDefault();

        if (target != null)
        {
            int damage = CalculatePhysicalDamage(beast.Stats.PhysAtk, target.Stats.PhysDef);
            target.TakeDamage(damage);

            _view.WriteLine($"{target.Name} recibe {damage} de daño físico");
            _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
        }
    }

    private int CalculatePhysicalDamage(int attackerPhysAtk, int targetPhysDef)
    {
        int damage = (int)Math.Floor((attackerPhysAtk * 1.3) - targetPhysDef);
        return Math.Max(0, damage);
    }

    private void PrintGameState()
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Equipo del jugador");
        for (int i = 0; i < _playerTeam.Count; i++)
        {
            var t = _playerTeam[i];
            char letter = (char)('A' + i);
            _view.WriteLine($"{letter}-{t.Name} - HP:{t.CurrentHP}/{t.Stats.HP} SP:{t.CurrentSP}/{t.Stats.SP} BP:{t.CurrentBP}");
        }

        _view.WriteLine("Equipo del enemigo");
        for (int i = 0; i < _enemyTeam.Count; i++)
        {
            var b = _enemyTeam[i];
            char letter = (char)('A' + i);
            _view.WriteLine($"{letter}-{b.Name} - HP:{b.CurrentHP}/{b.Stats.HP} Shields:{b.Shields}");
        }
    }

    private List<Unit> GenerateTurnQueue()
    {
        return _playerTeam.Cast<Unit>().Select(u => new { Unit = u, IsTraveler = true, Index = _playerTeam.IndexOf((Traveler)u) })
            .Concat(_enemyTeam.Cast<Unit>().Select(u => new { Unit = u, IsTraveler = false, Index = _enemyTeam.IndexOf((Beast)u) }))
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenByDescending(x => x.IsTraveler)
            .ThenBy(x => x.Index)
            .Select(x => x.Unit)
            .ToList();
    }

    private void PrintTurnOrder(List<Unit> queue, string title)
    {
        _view.WriteLine(Separator);
        _view.WriteLine(title);
        for (int i = 0; i < queue.Count; i++)
        {
            _view.WriteLine($"{i + 1}.{queue[i].Name}");
        }
    }

    private void PrintMessageWithSeparator(string message)
    {
        _view.WriteLine(Separator);
        _view.WriteLine(message);
    }

    private bool CheckWinCondition(out string message)
    {
        if (_enemyTeam.All(b => b.IsDead)) { message = "Gana equipo del jugador"; return true; }
        if (_playerTeam.All(t => t.IsDead)) { message = "Gana equipo del enemigo"; return true; }
        message = ""; return false;
    }
}