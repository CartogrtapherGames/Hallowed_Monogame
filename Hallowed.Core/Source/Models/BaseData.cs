namespace Hallowed.Core.Models;

[Serializable]
public abstract class BaseData
{
  public Guid Guid { get; set; } = Guid.NewGuid();
  public string Id  {get; set; } = string.Empty;
  
}
