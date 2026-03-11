using System.Text.Json;
using Octopath_Traveler.Models;

namespace Octopath_Traveler.Data
{
    public class JsonDataLoader
    {
        // Configuramos opciones para que el lector no sea estricto con mayúsculas/minúsculas
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonDataLoader()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public List<Traveler> LoadTravelers(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No se encontró el archivo de viajeros en: {filePath}");
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Traveler>>(jsonString, _jsonOptions);
        }

        public List<Beast> LoadBeasts(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No se encontró el archivo de bestias en: {filePath}");
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Beast>>(jsonString, _jsonOptions);
        }
        
        public List<string> LoadSkillNames(string filePath)
        {
            if (!File.Exists(filePath)) return new List<string>();

            string jsonString = File.ReadAllText(filePath);
            var skills = JsonSerializer.Deserialize<List<Skill>>(jsonString, _jsonOptions);
    
            // Usamos LINQ para extraer solo la propiedad "Name" de cada habilidad
            return skills.Select(s => s.Name).ToList();
        }
    }
}