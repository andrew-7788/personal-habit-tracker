using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HabitTracker;

public class FileSaver
{
    public void EnsureFileExists(string path)
    {
        string? directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(path))
        {
            using (File.Create(path))
            {
            }
        }
    }

    public List<string> ReadLines(string path)
    {
        EnsureFileExists(path);
        return File.ReadAllLines(path).ToList();
    }

    public void WriteLines(string path, List<string> lines)
    {
        EnsureFileExists(path);
        File.WriteAllLines(path, lines);
    }

    public void AppendLine(string path, string line)
    {
        EnsureFileExists(path);
        File.AppendAllLines(path, new[] { line });
    }
}