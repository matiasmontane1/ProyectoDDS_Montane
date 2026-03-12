using Octopath_Traveler_View;
using Octopath_Traveler.Models;

namespace Octopath_Traveler;

public class CombatManager
{
    private readonly View _view;
    private readonly List<Traveler> _playerTeam;
    private readonly List<Beast> _enemyTeam;
    private int _currentRound;

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
                _view.WriteLine("----------------------------------------");
                _view.WriteLine(winnerMessage);
                return; 
            }
            
            _view.WriteLine("----------------------------------------");
            _view.WriteLine($"INICIA RONDA {_currentRound}");
            
            // Generamos la cola inicial de la ronda
            var turnQueue = GenerateTurnQueue().Where(u => !u.IsDead).ToList();
            
            while (turnQueue.Count > 0)
            {
                if (CheckWinCondition(out string innerWinnerMessage))
                {
                    _view.WriteLine("----------------------------------------");
                    _view.WriteLine(innerWinnerMessage);
                    return; 
                }

                var currentUnit = turnQueue[0];

                if (currentUnit.IsDead) 
                {
                    turnQueue.RemoveAt(0);
                    continue;
                }

                PrintGameState();
                
                PrintTurnOrder(turnQueue, "Turnos de la ronda");
                
                var nextRoundQueue = GenerateTurnQueue().Where(u => !u.IsDead).ToList();
                PrintTurnOrder(nextRoundQueue, "Turnos de la siguiente ronda");

                if (currentUnit is Traveler traveler)
                {
                    bool fled = HandleTravelerTurn(traveler);
                    if (fled) return; 
                }
                else if (currentUnit is Beast beast)
                {
                    HandleBeastTurn(beast);
                }
                
                turnQueue.RemoveAt(0);
                
                turnQueue.RemoveAll(u => u.IsDead);
            }
            
            foreach (var traveler in _playerTeam.Where(t => !t.IsDead))
            {
                if (traveler.CurrentBP < 5) // El máximo de BP es 5
                {
                    traveler.RecoverBP();;
                }
            }

            _currentRound++;
        }
    }

    private bool HandleTravelerTurn(Traveler traveler)
    {
        while (true)
        {
            _view.WriteLine("----------------------------------------");
            _view.WriteLine($"Turno de {traveler.Name}");
            _view.WriteLine("1: Ataque básico");
            _view.WriteLine("2: Usar habilidad");
            _view.WriteLine("3: Defender");
            _view.WriteLine("4: Huir");

            string input = _view.ReadLine();

            if (input == "1")
            {
                if (ExecuteBasicAttack(traveler)) return false;
            }
            else if (input == "2")
            {
                HandleSkillMenu(traveler);
            }
            else if (input == "3")
            {
                return false; 
            }
            else if (input == "4")
            {
                _view.WriteLine("----------------------------------------");
                _view.WriteLine("El equipo de viajeros ha huido!");
                _view.WriteLine("----------------------------------------");
                _view.WriteLine("Gana equipo del enemigo");
                return true;
            }
        }
    }

    private bool ExecuteBasicAttack(Traveler traveler)
    {
        _view.WriteLine("----------------------------------------");
        _view.WriteLine("Seleccione un arma");
        for (int i = 0; i < traveler.Weapons.Count; i++)
        {
            _view.WriteLine($"{i + 1}: {traveler.Weapons[i]}");
        }
        _view.WriteLine($"{traveler.Weapons.Count + 1}: Cancelar");

        if (!int.TryParse(_view.ReadLine(), out int weaponChoice) || weaponChoice == traveler.Weapons.Count + 1)
            return false;

        string selectedWeapon = traveler.Weapons[weaponChoice - 1];

        _view.WriteLine("----------------------------------------");
        _view.WriteLine($"Seleccione un objetivo para {traveler.Name}");
        var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
        for (int i = 0; i < aliveEnemies.Count; i++)
        {
            _view.WriteLine($"{i + 1}: {aliveEnemies[i].Name} - HP:{aliveEnemies[i].CurrentHP}/{aliveEnemies[i].Stats.HP} Shields:{aliveEnemies[i].CurrentShields}");
        }
        _view.WriteLine($"{aliveEnemies.Count + 1}: Cancelar");

        if (!int.TryParse(_view.ReadLine(), out int targetChoice) || targetChoice == aliveEnemies.Count + 1)
            return false;

        var target = aliveEnemies[targetChoice - 1];

        if (traveler.CurrentBP >= 1)
        {
            _view.WriteLine("----------------------------------------");
            _view.WriteLine("Seleccione cuantos BP utilizar");
            _view.ReadLine(); 
        }

        int damage = (int)System.Math.Floor((traveler.Stats.PhysAtk * 1.3) - target.Stats.PhysDef);
        if (damage < 0) damage = 0;

        target.TakeDamage(damage);

        _view.WriteLine("----------------------------------------");
        _view.WriteLine($"{traveler.Name} ataca");
        _view.WriteLine($"{target.Name} recibe {damage} de daño de tipo {selectedWeapon}");
        _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");

        return true;
    }

    private void HandleSkillMenu(Traveler traveler)
    {
        _view.WriteLine("----------------------------------------"); 
        _view.WriteLine($"Seleccione una habilidad para {traveler.Name}");
        
        int count = 0;
        if (traveler.Skills != null)
        {
            for (int i = 0; i < traveler.Skills.Count; i++)
            {
                // Si la lista es de strings, esto funciona directo. 
                // Si es una lista de objetos Skill, quizás debas poner traveler.Skills[i].Name
                _view.WriteLine($"{i + 1}: {traveler.Skills[i]}");
                count++;
            }
        }

        _view.WriteLine($"{count + 1}: Cancelar");
        _view.ReadLine();
    }

    private void HandleBeastTurn(Beast beast)
    {
        _view.WriteLine("----------------------------------------");
        _view.WriteLine($"{beast.Name} usa {beast.Skill}");
        var target = _playerTeam.Where(t => !t.IsDead).OrderByDescending(t => t.CurrentHP).FirstOrDefault();

        if (target != null)
        {
            int damage = (int)System.Math.Floor((beast.Stats.PhysAtk * 1.3) - target.Stats.PhysDef);
            if (damage < 0) damage = 0;
            target.TakeDamage(damage);

            _view.WriteLine($"{target.Name} recibe {damage} de daño físico");
            _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
        }
    }

    private void PrintGameState()
    {
        _view.WriteLine("----------------------------------------");
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
        var allUnits = _playerTeam.Cast<Unit>().Select(u => new { Unit = u, IsTraveler = true, Index = _playerTeam.IndexOf((Traveler)u) })
            .Concat(_enemyTeam.Cast<Unit>().Select(u => new { Unit = u, IsTraveler = false, Index = _enemyTeam.IndexOf((Beast)u) }))
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenByDescending(x => x.IsTraveler)
            .ThenBy(x => x.Index)
            .Select(x => x.Unit)
            .ToList();
        return allUnits;
    }

    private void PrintTurnOrder(List<Unit> queue, string title)
    {
        _view.WriteLine("----------------------------------------");
        _view.WriteLine(title);
        for (int i = 0; i < queue.Count; i++)
        {
            _view.WriteLine($"{i + 1}.{queue[i].Name}");
        }
    }

    private bool CheckWinCondition(out string message)
    {
        if (_enemyTeam.All(b => b.IsDead)) { message = "Gana equipo del jugador"; return true; }
        if (_playerTeam.All(t => t.IsDead)) { message = "Gana equipo del enemigo"; return true; }
        message = ""; return false;
    }
}