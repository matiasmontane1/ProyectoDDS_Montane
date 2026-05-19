using Octopath_Traveler_View;

namespace Octopath_Traveler.Controllers;

public class GameView
{
    private readonly View _view;

    public GameView(View view) => _view = view;

    public string ReadLine() => _view.ReadLine();

    public void ShowNoTeamFilesFound()
    {
        _view.WriteLine("No se encontraron archivos de equipo.");
    }

    public void ShowTeamFilePrompt()
    {
        _view.WriteLine("Elige un archivo para cargar los equipos");
    }

    public void ShowFileOption(int index, string fileName)
    {
        _view.WriteLine($"{index}: {fileName}");
    }

    public void ShowInvalidSelection()
    {
        _view.WriteLine("Selección inválida o cancelada.");
    }
}
