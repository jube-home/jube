using System.IO;
using System.Reflection;

namespace Jube.Test;

public class Helpers
{
    public static string ReadFileContents(string filePath)
    {
        return File.ReadAllText(Path.Combine(
            GetParentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, 4) ?? string.Empty,
            $"Jube.Tests/{filePath}"));
    }

    private static string? GetParentDirectory(string? path, int parentCount)
    {
        if (string.IsNullOrEmpty(path) || parentCount < 1)
            return path;

        var parent = Path.GetDirectoryName(path);

        if (--parentCount > 0)
            if (parent != null)
                return GetParentDirectory(parent, parentCount);

        return parent;
    }
}