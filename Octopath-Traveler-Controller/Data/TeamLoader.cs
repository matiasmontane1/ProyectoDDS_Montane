using Octopath_Traveler.Models;

namespace Octopath_Traveler.Data;

public class TeamLoader
{
    private const int MinTravelerCount = 1;
    private const int MaxTravelerCount = 4;
    private const int MinBeastCount = 1;
    private const int MaxBeastCount = 5;
    private const int MaxActiveSkills = 8;
    private const int MaxPassiveSkills = 4;

    private readonly List<Traveler> _availableTravelers;
    private readonly List<Beast> _availableBeasts;
    private readonly List<string> _validActiveSkills;
    private readonly List<string> _validPassiveSkills;

    public TeamLoader(
        List<Traveler> availableTravelers, 
        List<Beast> availableBeasts,
        List<string> validActiveSkills, 
        List<string> validPassiveSkills)
    {
        _availableTravelers = availableTravelers;
        _availableBeasts = availableBeasts;
        _validActiveSkills = validActiveSkills;
        _validPassiveSkills = validPassiveSkills;
    }

    public (List<Traveler> PlayerTeam, List<Beast> EnemyTeam) LoadTeamFrom(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"No se encontró el archivo de equipos en: {filePath}");
        }

        string[] lines = File.ReadAllLines(filePath);
        var (playerTeam, enemyTeam) = ParseTeamsFromLines(lines);

        ValidateTeamSizes(playerTeam, enemyTeam);

        return (playerTeam, enemyTeam);
    }

    private (List<Traveler> PlayerTeam, List<Beast> EnemyTeam) ParseTeamsFromLines(string[] lines)
    {
        var cleanedLines = lines.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

        var playerLines = ExtractSection(cleanedLines, "Player Team", "Enemy Team");
        var enemyLines = ExtractSection(cleanedLines, "Enemy Team", null);

        var playerTeam = BuildPlayerTeam(playerLines);
        var enemyTeam = BuildEnemyTeam(enemyLines);

        return (playerTeam, enemyTeam);
    }

    private IEnumerable<string> ExtractSection(List<string> lines, string startHeader, string? stopHeader)
    {
        int startIndex = lines.IndexOf(startHeader);
        if (startIndex == -1) throw new InvalidDataException($"Falta la cabecera '{startHeader}'.");

        var section = lines.Skip(startIndex + 1);
        if (stopHeader != null)
        {
            int stopIndex = lines.IndexOf(stopHeader);
            section = section.Take(stopIndex - startIndex - 1);
        }

        return section;
    }

    private List<Traveler> BuildPlayerTeam(IEnumerable<string> lines)
    {
        var team = new List<Traveler>();
        var names = new HashSet<string>();
        foreach (var line in lines) AddTravelerToTeam(line, team, names);
        return team;
    }

    private List<Beast> BuildEnemyTeam(IEnumerable<string> lines)
    {
        var team = new List<Beast>();
        var names = new HashSet<string>();
        foreach (var line in lines) AddBeastToTeam(line, team, names);
        return team;
    }

    private void AddTravelerToTeam(string line, List<Traveler> team, HashSet<string> trackedNames)
    {
        Traveler parsedTraveler = ParseTraveler(line);

        if (!trackedNames.Add(parsedTraveler.Name))
        {
            throw new InvalidDataException($"El viajero '{parsedTraveler.Name}' está duplicado en el equipo.");
        }

        team.Add(parsedTraveler);
    }

    private void AddBeastToTeam(string beastName, List<Beast> team, HashSet<string> trackedNames)
    {
        Beast? baseBeast = _availableBeasts.FirstOrDefault(b => b.Name == beastName);

        if (baseBeast == null)
        {
            throw new InvalidDataException($"La bestia '{beastName}' no existe en la base de datos de JSON.");
        }

        if (!trackedNames.Add(baseBeast.Name))
        {
            throw new InvalidDataException($"La bestia '{baseBeast.Name}' está duplicada en el equipo.");
        }

        Beast newBeast = new Beast
        {
            Name = baseBeast.Name,
            Stats = baseBeast.Stats,
            Skill = baseBeast.Skill,
            Shields = baseBeast.Shields,
            Weaknesses = baseBeast.Weaknesses.ToList()
        };

        team.Add(newBeast);
    }

    private void ValidateTeamSizes(List<Traveler> playerTeam, List<Beast> enemyTeam)
    {
        if (playerTeam.Count < MinTravelerCount || playerTeam.Count > MaxTravelerCount)
            throw new InvalidDataException($"Cantidad de viajeros inválida ({playerTeam.Count}). Rango permitido: {MinTravelerCount}-{MaxTravelerCount}.");

        if (enemyTeam.Count < MinBeastCount || enemyTeam.Count > MaxBeastCount)
            throw new InvalidDataException($"Cantidad de bestias inválida ({enemyTeam.Count}). Rango permitido: {MinBeastCount}-{MaxBeastCount}.");
    }

    private Traveler ParseTraveler(string line)
    {
        string name = ExtractTravelerName(line);
        Traveler? baseTraveler = _availableTravelers.FirstOrDefault(t => t.Name == name);

        if (baseTraveler == null)
        {
            throw new InvalidDataException($"El viajero '{name}' no existe en la base de datos de JSON.");
        }

        var activeSkills = LoadActiveSkills(line);
        var passiveSkills = LoadPassiveSkills(line);

        return new Traveler
        {
            Name = baseTraveler.Name,
            Stats = baseTraveler.Stats,
            Weapons = baseTraveler.Weapons.ToList(),
            Skills = activeSkills,
            PassiveSkills = passiveSkills
        };
    }

    private string ExtractTravelerName(string line)
    {
        int firstParen = line.IndexOf('(');
        int firstBracket = line.IndexOf('[');

        int nameEndIndex = line.Length;
        if (firstParen != -1) nameEndIndex = Math.Min(nameEndIndex, firstParen);
        if (firstBracket != -1) nameEndIndex = Math.Min(nameEndIndex, firstBracket);

        return line.Substring(0, nameEndIndex).Trim();
    }

    private List<string> LoadActiveSkills(string line)
    {
        return ParseSkillsSection(line, '(', ')', MaxActiveSkills, _validActiveSkills);
    }

    private List<string> LoadPassiveSkills(string line)
    {
        return ParseSkillsSection(line, '[', ']', MaxPassiveSkills, _validPassiveSkills);
    }

    private List<string> ParseSkillsSection(string line, char open, char close, int maxAllowed, List<string> validDatabase)
    {
        string content = ExtractTextBetween(line, open, close);
        if (string.IsNullOrWhiteSpace(content)) return new List<string>();

        var skills = content.Split(',').Select(s => s.Trim()).ToList();
    
        ValidateParsedSkills(skills, maxAllowed, validDatabase, line);

        return skills;
    }

    private string ExtractTextBetween(string line, char open, char close)
    {
        int openIndex = line.IndexOf(open);
        if (openIndex == -1) return string.Empty;

        int closeIndex = line.IndexOf(close);
        if (closeIndex == -1 || closeIndex < openIndex)
            throw new InvalidDataException($"Error de sintaxis. Se esperaba '{close}' en la línea: {line}");

        return line.Substring(openIndex + 1, closeIndex - openIndex - 1);
    }

    private void ValidateParsedSkills(List<string> skills, int maxAllowed, List<string> validDatabase, string originalLine)
    {
        if (skills.Count > maxAllowed)
            throw new InvalidDataException($"Se excedió el límite de habilidades ({maxAllowed}) en: {originalLine}");

        if (skills.Count != skills.Distinct().Count())
            throw new InvalidDataException($"Habilidades duplicadas detectadas en: {originalLine}");

        var invalidSkills = skills.Where(skill => !validDatabase.Contains(skill)).ToList();
        if (invalidSkills.Any())
            throw new InvalidDataException($"Habilidades no reconocidas: {string.Join(", ", invalidSkills)}");
    }
}