namespace Data;

public interface IFileRepository<T>
{
    Task<List<T>> ReadDataFromFile(string filePath);
   
    Task WriteDataToFile(string filePath, List<T> data);
}