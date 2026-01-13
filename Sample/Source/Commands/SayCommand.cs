using System;
using System.Threading.Tasks;

namespace Sample.Source.Commands;

[Command("say")]
public class SayCommand : ScriptCommandBase
{

  [CommandParameter(Name = "text", Optional = false)]
  public string Text { get; private set; }
  

  protected override async Task ExecuteAsync()
  {
    Console.WriteLine(Text);
  }
  
  
}
