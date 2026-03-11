using Octopath_Traveler_View;
using Octopath_Traveler.Data;

namespace Octopath_Traveler;

public class Game
{
    private View view;
    private string teamsFolder;
    public Game(View view, string teamsFolder)
    {
        this.view = view;
        this.teamsFolder = teamsFolder;
    }

    public void Play()
    {
        // 1. Mostrar las opciones de equipos
        view.WriteLine("Elige un archivo para cargar los equipos");
        
        // Obtenemos los archivos .txt y los ordenamos alfabéticamente
        string[] files = Directory.GetFiles(teamsFolder, "*.txt").OrderBy(f => f).ToArray();
        
        for (int i = 0; i < files.Length; i++)
        {
            view.WriteLine($"{i}: {Path.GetFileName(files[i])}");
        }

        // 2. Pedir el input al usuario (recuerda, NO imprimimos "INPUT:")
        string input = view.ReadLine();
        
        // 3. Validar que el input sea un número válido y esté en el rango
        if (!int.TryParse(input, out int selectedIndex) || selectedIndex < 0 || selectedIndex >= files.Length)
        {
            view.WriteLine("Archivo de equipos no válido");
            return; // Termina el juego
        }

        string selectedFile = files[selectedIndex];

        // 4. Cargar la "Base de Datos" desde los JSON
        var jsonLoader = new JsonDataLoader();
        // Asumimos que la carpeta data está en el directorio de ejecución
        var characters = jsonLoader.LoadTravelers("data/characters.json");
        var enemies = jsonLoader.LoadBeasts("data/enemies.json");
        var activeSkills = jsonLoader.LoadSkillNames("data/skills.json");
        var passiveSkills = jsonLoader.LoadSkillNames("data/passive_skills.json");

        // 5. Validar el equipo usando nuestra lógica Clean Code
        var teamLoader = new TeamLoader();
        var teams = teamLoader.LoadTeam(selectedFile, characters, enemies, activeSkills, passiveSkills);

        if (teams == null)
        {
            // Si el teamLoader devolvió null, es porque rompió alguna regla del enunciado
            view.WriteLine("Archivo de equipos no válido");
            return; // Termina el juego
        }
    }
}