using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public static class JsonDatabase
{
    public static async Task<List<T>> ReadAsync<T>(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return new List<T>();
        }

        var json = await File.ReadAllTextAsync(fileName);

        return string.IsNullOrEmpty(json) ? new List<T>() : JsonSerializer.Deserialize<List<T>>(json);
    }

    public static async Task WriteAsync<T>(string fileName, List<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(fileName, json);
    }
}