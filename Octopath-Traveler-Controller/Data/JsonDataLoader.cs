using System.Text.Json;

namespace Octopath_Traveler.Data;

public class JsonDataLoader
{
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonDataLoader()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public T Load<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Error crítico: No se encontró el archivo de datos en: {filePath}");
        }

        string jsonString = File.ReadAllText(filePath);
        var result = JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);

        if (result == null)
        {
            throw new InvalidDataException($"El archivo {filePath} está vacío o su formato es inválido.");
        }

        return result;
    }
}