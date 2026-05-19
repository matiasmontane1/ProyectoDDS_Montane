using Octopath_Traveler_View;
using Octopath_Traveler.Controllers;
using Octopath_Traveler.Data;
using Octopath_Traveler.Models;

namespace Octopath_Traveler;

public class Game
{
    private const string CharactersPath = "data/characters.json";
    private const string EnemiesPath = "data/enemies.json";
    private const string ActiveSkillsPath = "data/skills.json";
    private const string BeastSkillsPath = "data/beast_skills.json";
    private const string PassiveSkillsPath = "data/passive_skills.json";

    private readonly View _view;
    private readonly GameView _gameView;
    private readonly string _teamsFolder;
    private readonly JsonDataLoader _jsonLoader;

    public Game(View view, string teamsFolder)
    {
        _view = view;
        _gameView = new GameView(view);
        _teamsFolder = teamsFolder;
        _jsonLoader = new JsonDataLoader();
    }

    public void Play()
    {
        string? selectedFile = GetTeamFileFromUser();
        if (selectedFile == null) return;

        try
        {
            var data = LoadGameData();
            var teams = LoadTeams(selectedFile, data);

            StartCombat(teams, data);
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is InvalidDataException)
        {
            _view.WriteLine("Archivo de equipos no válido");
        }
    }

    private string? GetTeamFileFromUser()
    {
        var files = GetAvailableTeamFiles();
        if (!files.Any())
        {
            _gameView.ShowNoTeamFilesFound();
            return null;
        }

        DisplayFileOptions(files);
        return ReadAndValidateUserSelection(files);
    }

    private string[] GetAvailableTeamFiles()
    {
        if (!Directory.Exists(_teamsFolder)) return Array.Empty<string>();
        return Directory.GetFiles(_teamsFolder, "*.txt").OrderBy(file => file).ToArray();
    }

    private void DisplayFileOptions(string[] files)
    {
        _gameView.ShowTeamFilePrompt();
        for (int index = 0; index < files.Length; index++)
        {
            _gameView.ShowFileOption(index, Path.GetFileName(files[index]));
        }
    }

    private string? ReadAndValidateUserSelection(string[] files)
    {
        string input = _gameView.ReadLine();

        if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 0 && selectedIndex < files.Length)
        {
            return files[selectedIndex];
        }

        _gameView.ShowInvalidSelection();
        return null;
    }

    private (List<ActiveSkill> ActiveSkills, List<BeastSkill> BeastSkills) LoadGameData()
    {
        var activeSkills = _jsonLoader.Load<List<ActiveSkill>>(ActiveSkillsPath);
        var beastSkills = _jsonLoader.Load<List<BeastSkill>>(BeastSkillsPath);
        return (activeSkills, beastSkills);
    }

    private (List<Traveler> PlayerTeam, List<Beast> EnemyTeam) LoadTeams(
        string selectedFile,
        (List<ActiveSkill> ActiveSkills, List<BeastSkill> BeastSkills) data)
    {
        var characters = _jsonLoader.Load<List<Traveler>>(CharactersPath);
        var enemies = _jsonLoader.Load<List<Beast>>(EnemiesPath);
        var passiveSkillsData = _jsonLoader.Load<List<Skill>>(PassiveSkillsPath);

        var activeSkillNames = data.ActiveSkills.Select(skill => skill.Name).ToList();
        var passiveSkillNames = passiveSkillsData.Select(skill => skill.Name).ToList();

        var teamLoader = new TeamLoader(characters, enemies, activeSkillNames, passiveSkillNames);
        return teamLoader.LoadTeamFrom(selectedFile);
    }

    private void StartCombat(
        (List<Traveler> PlayerTeam, List<Beast> EnemyTeam) teams,
        (List<ActiveSkill> ActiveSkills, List<BeastSkill> BeastSkills) data)
    {
        var combatManager = new CombatManager(
            _view,
            teams.PlayerTeam,
            teams.EnemyTeam,
            data.ActiveSkills,
            data.BeastSkills
        );

        combatManager.StartCombat();
    }
}
