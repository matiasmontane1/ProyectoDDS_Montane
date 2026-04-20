using Octopath_Traveler_View;
using Octopath_Traveler.Data;
using Octopath_Traveler.Models;

namespace Octopath_Traveler;

public class Game
{
    private readonly View _view;
    private readonly string _teamsFolder;

    public Game(View view, string teamsFolder)
    {
        _view = view;
        _teamsFolder = teamsFolder;
    }

    public void Play()
    {
        string selectedFile = PromptTeamSelection();
        if (selectedFile == null)
        {
            _view.WriteLine("Archivo de equipos no válido");
            return;
        }

        var data = LoadGameData();
        var teams = LoadTeams(selectedFile, data);
        if (teams == null)
        {
            _view.WriteLine("Archivo de equipos no válido");
            return;
        }

        var combatManager = new CombatManager(
            _view,
            teams.Value.playerTeam,
            teams.Value.enemyTeam,
            data.activeSkills,
            data.beastSkills
        );
        combatManager.StartCombat();
    }

    private string PromptTeamSelection()
    {
        _view.WriteLine("Elige un archivo para cargar los equipos");

        string[] files = Directory.GetFiles(_teamsFolder, "*.txt").OrderBy(f => f).ToArray();

        for (int i = 0; i < files.Length; i++)
            _view.WriteLine($"{i}: {Path.GetFileName(files[i])}");

        string input = _view.ReadLine();

        if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 0 && selectedIndex < files.Length)
            return files[selectedIndex];

        return null;
    }

    private (List<ActiveSkill> activeSkills, List<BeastSkill> beastSkills) LoadGameData()
    {
        var jsonLoader = new JsonDataLoader();
        var activeSkills = jsonLoader.LoadActiveSkills("data/skills.json");
        var beastSkills = jsonLoader.LoadBeastSkills("data/beast_skills.json");
        return (activeSkills, beastSkills);
    }

    private (List<Traveler> playerTeam, List<Beast> enemyTeam)? LoadTeams(
        string selectedFile,
        (List<ActiveSkill> activeSkills, List<BeastSkill> beastSkills) data)
    {
        var jsonLoader = new JsonDataLoader();
        var characters = jsonLoader.LoadTravelers("data/characters.json");
        var enemies = jsonLoader.LoadBeasts("data/enemies.json");
        var activeSkillNames = data.activeSkills.Select(s => s.Name).ToList();
        var passiveSkillNames = jsonLoader.LoadSkillNames("data/passive_skills.json");

        var teamLoader = new TeamLoader(characters, enemies, activeSkillNames, passiveSkillNames);
        return teamLoader.LoadTeamFrom(selectedFile);
    }
}
