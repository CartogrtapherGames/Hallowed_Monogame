using System.IO;
using Hallowed.Utilities;

namespace Sample;

public class Paths(string value) : SmartEnum<Paths, string>(value)
{

  public static readonly string GumProject = new Paths("GumProjects/MainProject.gumx").Value;
  public static readonly string TitleScreen = new Paths("TitleScreen").Value;
}
