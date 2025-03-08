using System.Text.Json;

namespace Data.Files;

public class FileRepository<T> : IFileRepository<T>
{
    
    public async Task<List<T>> ReadDataFromFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return [];
            }

            var jsonString = await File.ReadAllTextAsync(filePath);
            return (!string.IsNullOrWhiteSpace(jsonString) ? JsonSerializer.Deserialize<List<T>>(jsonString) : []) ??
                   [];
        }
        catch (FileNotFoundException exp)
        {
            Console.WriteLine($"File {filePath} not found. {exp.Message}");
            throw;
        }
    }

    public async Task WriteDataToFile(string filePath, List<T> data)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return ;
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true  
            };
            var jsonArray = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(filePath, jsonArray);
        }
        catch(FileNotFoundException exp)
        {
            Console.WriteLine($"File {filePath} not found. {exp.Message}");
            throw;
        }
    }
}