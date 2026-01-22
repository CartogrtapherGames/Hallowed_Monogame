using System;
using System.IO;
using System.Threading.Tasks;

namespace Hallowed.Editor.Services;

public static class ProjectManager
{
  /// <summary>
  /// Path to the current project folder (e.g., C:\Projects\MyGame)
  /// Empty string if no project is open.
  /// </summary>
  public static string ProjectPath { get; set; } = "";

  /// <summary>
  /// Gets the project name from the path.
  /// </summary>
  public static string ProjectName => "";

  /// <summary>
  /// Checks if a project is currently open.
  /// </summary>
  public static bool IsProjectOpen => !string.IsNullOrEmpty(ProjectPath);

  /// <summary>
  /// Opens a project from a .hlwproj file.
  /// </summary>
  public static async Task<bool> OpenProjectFromFile(string hlwprojFilePath)
  {
    try
    {
      // Validate the .hlwproj file exists
      if (!File.Exists(hlwprojFilePath))
      {
        Console.WriteLine($"Project file not found: {hlwprojFilePath}");
        return false;
      }

      // Get the project folder from the .hlwproj file's directory
      ProjectPath = Path.GetDirectoryName(hlwprojFilePath) ?? "";

      if (string.IsNullOrEmpty(ProjectPath) || !Directory.Exists(ProjectPath))
      {
        Console.WriteLine($"Project folder not found: {ProjectPath}");
        ProjectPath = "";
        return false;
      }

      Console.WriteLine($"Project opened: {ProjectPath}");
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Failed to open project: {ex.Message}");
      ProjectPath = "";
      return false;
    }
  }

  /// <summary>
  /// Creates a new project and saves the .hlwproj file.
  /// </summary>
  public static async Task<bool> CreateProject(string hlwprojFilePath, string projectFolderPath)
  {
    try
    {
      // Create project folder if it doesn't exist
      Directory.CreateDirectory(projectFolderPath);

      // Write project folder path to .hlwproj file
      await File.WriteAllTextAsync(hlwprojFilePath, projectFolderPath);

      ProjectPath = projectFolderPath;

      Console.WriteLine($"Project created: {ProjectPath}");
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Failed to create project: {ex.Message}");
      return false;
    }
  }

  /// <summary>
  /// Closes the current project.
  /// </summary>
  public static void CloseProject()
  {
    ProjectPath = "";
    Console.WriteLine("Project closed");
  }

  /// <summary>
  /// Gets the full path for a data file within the project.
  /// </summary>
  public static string GetDataPath(string filename)
  {
    if (!IsProjectOpen)
      throw new InvalidOperationException("No project is open");

    var path = Path.Combine(ProjectPath, "Content/Data");
    return Path.Combine(path, filename);
  }

  /// <summary>
  /// Checks if a data file exists in the project.
  /// </summary>
  public static bool DataFileExists(string filename)
  {
    if (!IsProjectOpen) return false;
    return File.Exists(GetDataPath(filename));
  }
}
