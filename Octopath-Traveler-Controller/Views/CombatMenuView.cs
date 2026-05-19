using Octopath_Traveler.Models;
using Octopath_Traveler_View;

namespace Octopath_Traveler.Views;

public class CombatMenuView
{
    private readonly View _view;
    private const string Separator = "----------------------------------------";

    public CombatMenuView(View view) => _view = view;

    public string ReadLine() => _view.ReadLine();

    public void ShowTravelerActionMenu(string travelerName)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Turno de {travelerName}");
        _view.WriteLine("1: Ataque básico");
        _view.WriteLine("2: Usar habilidad");
        _view.WriteLine("3: Defender");
        _view.WriteLine("4: Huir");
    }

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
            var beast = targets[i];
            _view.WriteLine($"{i + 1}: {beast.Name} - HP:{beast.CurrentHp}/{beast.Stats.Hp} Shields:{beast.CurrentShields}");
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
            var traveler = targets[i];
            _view.WriteLine($"{i + 1}: {traveler.Name} - HP:{traveler.CurrentHp}/{traveler.Stats.Hp} SP:{traveler.CurrentSp}/{traveler.Stats.Sp} BP:{traveler.CurrentBp}");
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
