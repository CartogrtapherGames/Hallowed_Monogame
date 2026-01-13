using System.Threading.Tasks;

namespace Sample.Source.Commands;

public abstract class ScriptCommandBase
{
  
  public bool IsCompleted { get; private set; }
  protected ScriptContext Context { get; set; }
  
  public async Task RunAsync(ScriptContext context)
  {
    IsCompleted = false;
    Context = context;
    try
    {
      await ExecuteAsync();
    }
    finally
    {
      IsCompleted = true;
    }
  }
  
  protected abstract Task ExecuteAsync();
}
