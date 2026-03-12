using System.Text.Json;
using Octopath_Traveler.Models;

namespace Octopath_Traveler.Data
{
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

        private T DeserializeFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No se encontró el archivo JSON en: {filePath}");
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
        }

        public List<Traveler> LoadTravelers(string filePath)
        {
            return DeserializeFromFile<List<Traveler>>(filePath);
        }

        public List<Beast> LoadBeasts(string filePath)
        {
            return DeserializeFromFile<List<Beast>>(filePath);
        }
        
        public List<string> LoadSkillNames(string filePath)
        {
            if (!File.Exists(filePath)) 
            {
                return new List<string>();
            }

            var skills = DeserializeFromFile<List<Skill>>(filePath);
            return skills.Select(s => s.Name).ToList();
        }
    }
}