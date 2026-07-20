using System.Text.Json;

namespace WorldCupMatchTrackerRedesigned;

public class JsonStorageService
{
    private readonly string filePath = "matches.json";

    public List<Match> LoadMatches()
    {
        if (!File.Exists(filePath))
        {
            return new List<Match>();
        }

        string json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<List<Match>>(json) ?? new List<Match>();
    }

    public void SaveMatches(List<Match> matches)
    {
        string json = JsonSerializer.Serialize(matches, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
    }
}