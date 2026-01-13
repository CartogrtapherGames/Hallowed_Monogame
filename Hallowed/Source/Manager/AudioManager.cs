using Hallowed.Core;
using Microsoft.Xna.Framework;

namespace Hallowed.Manager;

public class AudioManager : IGameComponent
{

  public void Initialize()
  {
    throw new System.NotImplementedException();
  }

  public void QueueLoad()
  {
    
  }
  public void PlayBgm(string name, int volume = 100, int pitch = 100, int pan = 0, float pos = 0f)
  {
    var obj = new AudioObject()
    {
      name = name, 
      volume = volume, 
      pitch = pitch, 
      pan = pan, 
      pos = pos
    }; 
    PlayBgm(obj);
  }
  
  public void PlayBgm(AudioObject audioObject)
  {
    
  }
}
