using System;
using System.Reflection;

namespace Sample.Source.Commands;

/// <summary>
/// the static class that will scan the assemblies for the visual novel commands.
/// </summary>
public static class CommandScanner
{
  
  /// <summary>
  /// Scan the assemblies and assign commands to the registry.
  /// </summary>
  /// <param name="registry"></param>
  public static void ScanAndRegister(CommandRegistry registry)
  {
    
    var entryAssembly = Assembly.GetEntryAssembly();

    if (entryAssembly != null)
    {
      Console.WriteLine($"Scanning assembly {entryAssembly.FullName}");
      ScanAssembly(registry, entryAssembly);
    }
    
    var executingAssembly = Assembly.GetExecutingAssembly();

    if (executingAssembly == entryAssembly) return;
    Console.WriteLine($"Scanning assembly {executingAssembly.FullName}");
    ScanAssembly(registry, executingAssembly);
  }

  static void ScanAssembly(CommandRegistry registry, Assembly assembly)
  {
    try
    {
      var types = assembly.GetTypes();
      foreach (var type in types)
      {
        if (!type.IsClass) continue;
        if (type.IsAbstract) continue;

        var commandAttr = type.GetCustomAttribute<CommandAttribute>();
        if (commandAttr is null) continue;

        if (!typeof(ScriptCommandBase).IsAssignableFrom(type))
        {
          Console.WriteLine($"Error: Class '{type.Name}' has [Command] attribute but doesn't inherit from ScriptCommandBase");
          continue;
        }
        if (type.GetConstructor(Type.EmptyTypes) == null)
        {
          Console.WriteLine($"Error: Command class '{type.Name}' must have a parameterless constructor.");
          continue;
        }
        registry.Register(commandAttr.Name, type);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error scanning assembly {assembly.FullName}: {ex.Message}");
    }
  }
}
