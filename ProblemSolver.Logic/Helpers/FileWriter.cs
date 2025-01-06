namespace ProblemSolver.Logic.Helpers;

/// <summary>
///     Just for an easy way of saving any data to files.
/// </summary>
public static class FileWriter
{
    public static async Task WriteToFileAsync(string folder, string fileName, string content)
    {
        string filePath =
            $"{folder}/{fileName}";

        if (File.Exists(filePath))
            File.Delete(filePath);

        Directory.CreateDirectory(folder);
        File.Create(filePath).Close();

        await File.WriteAllTextAsync(filePath, content);
    }
}