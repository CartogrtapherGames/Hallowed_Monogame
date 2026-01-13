using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sample.Source.Commands;

/// <summary>
/// The command registry that store the registered commands.
/// </summary>
public class CommandRegistry
{

    readonly Dictionary<string, Type> _commands = new Dictionary<string, Type>();


  /// <summary>
  /// register a new command.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="commandType"></param>
  public void Register(string name, Type commandType)
  {
    if(HasCommand(name)) throw new ArgumentException($"The command {name} is already registered.");
    ArgumentNullException.ThrowIfNull(commandType);
    var key = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(name).Replace(" ", string.Empty);
    _commands.Add(key, commandType);
  }

  /// <summary>
  /// get the command instance.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public Type GetCommandType(string name)
  {
    _commands.TryGetValue(name, out var command);
    return command;
  }

  /// <summary>
  /// create the command instance.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="namedArgs"></param>
  /// <param name="positionalArgs"></param>
  /// <returns></returns>
  public ScriptCommandBase CreateCommand(string name, Dictionary<string, object> namedArgs, List<object> positionalArgs)
    {
        var type = GetCommandType(name);
        if (type == null) return null;

        // 1. Create Instance (Parameterless)
        var command = (ScriptCommandBase)Activator.CreateInstance(type);

        // 2. Get all properties marked with [CommandParameter]
        var properties = type.GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(CommandParameterAttribute)))
            .ToList();

        // 3. Fill Properties from Arguments
        int positionalIndex = 0;

        foreach (var prop in properties)
        {
            var attr = prop.GetCustomAttribute<CommandParameterAttribute>();
            var paramName = attr?.Name ?? prop.Name; // Use property name if alias not provided

            object valueToSet;

            // A. Try Named Argument (e.g. duration=5.0)
            // Case-insensitive match
            var matchingKey = namedArgs.Keys.FirstOrDefault(k => string.Equals(k, paramName, StringComparison.OrdinalIgnoreCase));
            
            if (matchingKey != null)
            {
                valueToSet = namedArgs[matchingKey];
            }
            // B. Try Positional Argument (e.g. @wait 5.0)
            else if (positionalIndex < positionalArgs.Count)
            {
                valueToSet = positionalArgs[positionalIndex];
                positionalIndex++;
            }
            // C. Use Default or Error
            else
            {
                if (attr is { Optional: true })
                {
                    valueToSet = attr.DefaultValue;
                }
                else
                {
                    throw new ArgumentException($"Missing required argument '{paramName}' for command '{name}'");
                }
            }

            // 4. Set the Value (with Type Conversion)
            if (valueToSet == null) continue;
            try 
            {
                // Convert "5" (string) to 5.0 (float) automatically
                var convertedValue = Convert.ChangeType(valueToSet, prop.PropertyType);
                prop.SetValue(command, convertedValue);
            }
            catch
            {
                Console.WriteLine($"Failed to convert '{valueToSet}' to {prop.PropertyType.Name} for {paramName}");
            }
        }

        return command;
    }
  
  /// <summary>
  /// Checks if the registry has a command with the specified name.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  public bool HasCommand(string name)
  {
    var key = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(name).Replace(" ", string.Empty);
    return !string.IsNullOrWhiteSpace(key) && _commands.ContainsKey(key);
  }
  
  public IEnumerable<string> GetAllCommandsNames() => _commands.Keys;
  
  public int Count => _commands.Count;
}
