using System;
using Microsoft.Xna.Framework;
using Sample.Source.Commands;

namespace Sample.Source;

public class ScriptInterpreter
{

  CommandRegistry _commandRegistry;

  public ScriptInterpreter()
  {
    _commandRegistry = new CommandRegistry();
    Console.WriteLine("ScriptInterpreter initialized");
    CommandScanner.ScanAndRegister( _commandRegistry);
    Console.WriteLine("CommandRegistry registered!");

  //  var say = _commandRegistry.CreateCommand("say", "Hello World!");
  
  }


  public void Update(GameTime gameTime)
  {
  }
}
