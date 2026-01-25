namespace Hallowed.Core.Models.Objects;

public class GameItem
{
  public string Id {get; private set;}
  public string Name { get; set; }
  public string Description { get; set; }
  public bool Consumable { get; set; }
  public string Icon { get; set; }
  public int Amount;
}
