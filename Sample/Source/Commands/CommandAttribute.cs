using System;

namespace Sample.Commands;

[AttributeUsage( AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
  
  public string Name { get; set; }

  public CommandAttribute(string name)
  {
    Name = name;
  }
}
