using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample.Source;

/// <summary>
/// The class That allow commands to access the context of the script.
/// </summary>
public class ScriptContext
{

  Dictionary<string, IContext> _context;


  public ScriptContext()
  {
    _context = new Dictionary<string, IContext>();
  }

  public void Register(string name, IContext context)
  {
    context.Parent = this;
    if (!_context.TryAdd(name, context))
    {
      throw new Exception($"The context {name} already exists.");
    }
  }

  public T Get<T>(string name) where T : IContext
  {
    if (_context != null && !_context.ContainsKey(name))
    {
      throw new Exception($"The context {name} does not exist.");
    }
    return (T)_context?[name];
  }

  public T Get<T>() where T : IContext
  {
    var match = _context.Values.OfType<T>().FirstOrDefault();
    return match ?? throw new KeyNotFoundException($"No context of type {typeof(T).Name} was found.");
  }
}