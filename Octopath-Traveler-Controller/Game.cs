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

        var teams = LoadGameDataAndTeams(selectedFile);
        if (teams == null)
        {
            _view.WriteLine("Archivo de equipos no válido");
            return; 
        }

        var combatManager = new CombatManager(_view, teams.Value.playerTeam, teams.Value.enemyTeam);
        combatManager.StartCombat();
    }

    private string PromptTeamSelection()
    {
        _view.WriteLine("Elige un archivo para cargar los equipos");
        
        string[] files = Directory.GetFiles(_teamsFolder, "*.txt").OrderBy(f => f).ToArray();
        
        for (int i = 0; i < files.Length; i++)
        {
            _view.WriteLine($"{i}: {Path.GetFileName(files[i])}");
        }

        string input = _view.ReadLine();
        
        if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 0 && selectedIndex < files.Length)
        {
            return files[selectedIndex];
        }

        return null; // Retornamos null si el usuario se equivocó
    }

    private (List<Traveler> playerTeam, List<Beast> enemyTeam)? LoadGameDataAndTeams(string selectedFile)
    {
        var jsonLoader = new JsonDataLoader();
        var characters = jsonLoader.LoadTravelers("data/characters.json");
        var enemies = jsonLoader.LoadBeasts("data/enemies.json");
        var activeSkills = jsonLoader.LoadSkillNames("data/skills.json");
        var passiveSkills = jsonLoader.LoadSkillNames("data/passive_skills.json");

        var teamLoader = new TeamLoader();
        return teamLoader.LoadTeam(selectedFile, characters, enemies, activeSkills, passiveSkills);
    }
}