using System;

namespace Sample.Source.Commands;

[AttributeUsage(AttributeTargets.Property)]
public class CommandParameterAttribute : Attribute
{
  public string Name { get; set; }
  public bool Optional { get; set; } = false;
  public object DefaultValue { get; set; }
}
