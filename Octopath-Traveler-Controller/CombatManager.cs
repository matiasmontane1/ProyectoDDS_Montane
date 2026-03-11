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

        // Inicializamos el HP y stats base de todas las unidades al empezar
        foreach (var traveler in _playerTeam) traveler.InitializeState();
        foreach (var beast in _enemyTeam) beast.InitializeState();
    }

    public void StartCombat()
    {
        while (true)
        {
            _view.WriteLine($"INICIA RONDA {_currentRound}");
            
            // Verificamos quién está vivo para armar la cola
            var turnQueue = GenerateTurnQueue().Where(u => !u.IsDead).ToList();
            
            // Antes del turno de cada unidad se muestra el estado y las colas
            foreach (var unit in turnQueue)
            {
                // Si alguien ganó, se acaba el combate inmediatamente
                if (CheckWinCondition(out string winnerMessage))
                {
                    _view.WriteLine(winnerMessage);
                    return; 
                }

                if (unit.IsDead) continue; // Si la unidad murió en esta misma ronda, pierde su turno

                PrintGameState();
                PrintTurnOrder(turnQueue, "Turnos de la ronda");
                PrintTurnOrder(turnQueue, "Turnos de la siguiente ronda");

                if (unit is Traveler traveler)
                {
                    bool fled = HandleTravelerTurn(traveler);
                    if (fled) return; // Si huyó, el combate termina inmediatamente
                }
                else if (unit is Beast beast)
                {
                    HandleBeastTurn(beast);
                }
                
                // Quitamos a la unidad que ya jugó de la cola de la ronda actual para la próxima impresión
                turnQueue.RemoveAt(0);
            }

            _currentRound++;
        }
    }

    private bool HandleTravelerTurn(Traveler traveler)
    {
        while (true) // Bucle por si el jugador elige "Cancelar" en algún submenú
        {
            _view.WriteLine($"Turno de {traveler.Name}");
            _view.WriteLine("1: Ataque básico");
            _view.WriteLine("2: Usar habilidad");
            _view.WriteLine("3: Defender");
            _view.WriteLine("4: Huir");

            string input = _view.ReadLine();

            if (input == "1")
            {
                // TODO: Implementar Ataque Básico en el próximo paso
                return false; 
            }
            else if (input == "2")
            {
                // TODO: Implementar menú de habilidades (siempre cancelarán en esta entrega)
            }
            else if (input == "3")
            {
                // TODO: Defender (no se evalúa en esta entrega, pero hay que tener la opción)
            }
            else if (input == "4")
            {
                _view.WriteLine("El equipo de viajeros ha huido!");
                _view.WriteLine("Gana equipo del enemigo");
                return true; // Retorna true indicando que huyó
            }
        }
    }

    private void HandleBeastTurn(Beast beast)
    {
        // TODO: Implementar ataque automático de la bestia
        _view.WriteLine($"{beast.Name} usa {beast.Skill}");
    }

    private bool CheckWinCondition(out string message)
    {
        bool allTravelersDead = _playerTeam.All(t => t.IsDead);
        bool allBeastsDead = _enemyTeam.All(b => b.IsDead);

        if (allBeastsDead)
        {
            message = "Gana equipo del jugador";
            return true;
        }
        
        if (allTravelersDead)
        {
            message = "Gana equipo del enemigo";
            return true;
        }

        message = string.Empty;
        return false;
    }

    private void PrintGameState()
    {
        _view.WriteLine("Equipo del jugador");
        for (int i = 0; i < _playerTeam.Count; i++)
        {
            var t = _playerTeam[i];
            char letter = (char)('A' + i); // Convierte 0 en 'A', 1 en 'B', etc.
            _view.WriteLine($"{letter}-{t.Name} HP: {t.CurrentHP}/{t.Stats.HP} SP: {t.CurrentSP}/{t.Stats.SP} BP: {t.CurrentBP}");
        }

        _view.WriteLine("Equipo del enemigo");
        for (int i = 0; i < _enemyTeam.Count; i++)
        {
            var b = _enemyTeam[i];
            char letter = (char)('A' + i);
            _view.WriteLine($"{letter}-{b.Name} HP: {b.CurrentHP}/{b.Stats.HP} Shields: {b.CurrentShields}");
        }
    }

    private List<Unit> GenerateTurnQueue()
    {
        // Guardamos la unidad, si es viajero (para desempates), y su posición en el tablero
        var allUnits = new List<(Unit unit, bool isTraveler, int boardIndex)>();
        
        for (int i = 0; i < _playerTeam.Count; i++)
            allUnits.Add((_playerTeam[i], true, i));
            
        for (int i = 0; i < _enemyTeam.Count; i++)
            allUnits.Add((_enemyTeam[i], false, i));

        // Reglas estrictas de ordenamiento de la cola de turnos
        return allUnits
            .OrderByDescending(u => u.unit.Stats.Speed) // 1. Mayor velocidad primero
            .ThenByDescending(u => u.isTraveler)        // 2. Empate: Viajeros sobre bestias
            .ThenBy(u => u.boardIndex)                  // 3. Empate: Orden de tablero (izquierda a derecha)
            .Select(u => u.unit)
            .ToList();
    }

    private void PrintTurnOrder(List<Unit> queue, string title)
    {
        _view.WriteLine(title);
        for (int i = 0; i < queue.Count; i++)
        {
            _view.WriteLine($"{i + 1}. {queue[i].Name}");
        }
    }
}