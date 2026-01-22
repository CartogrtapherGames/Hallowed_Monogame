using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Hallowed.Editor.ViewModels;

public partial class SystemViewModel : ObservableObject
{

  [ObservableProperty]
  int actorCount;

  [ObservableProperty]
  string author = "";

  [ObservableProperty]
  string currency = "Gold";

  [ObservableProperty]
  string description = "";

  // Game Options
  [ObservableProperty]
  bool enableAutosave = true;

  [ObservableProperty]
  bool enableCombat = true;

  [ObservableProperty]
  bool enableEquipment = true;

  [ObservableProperty]
  bool enableInventory = true;

  [ObservableProperty]
  bool enableQuests = true;

  [ObservableProperty]
  bool enableSkills = true;

  [ObservableProperty]
  int itemCount;

  [ObservableProperty]
  int maxPartySize = 4;
  [ObservableProperty]
  string projectName = "My Game Project";

  [ObservableProperty]
  string selectedResolution;

  [ObservableProperty]
  string selectedStartingScene;

  [ObservableProperty]
  string selectedWindowMode;

  [ObservableProperty]
  bool showMessageSkip = true;

  [ObservableProperty]
  bool skipTitleScreen = true;

  // Statistics
  [ObservableProperty]
  int storyNodeCount;

  [ObservableProperty]
  string version = "1.0.0";

  [ObservableProperty]
  bool vSyncEnabled = true;

  public SystemViewModel()
  {
    // Set defaults AFTER collections are initialized
    selectedResolution = Resolutions.FirstOrDefault();
    selectedWindowMode = WindowModes.FirstOrDefault();
    selectedStartingScene = StartingScenes.FirstOrDefault();

    Statistics = new ObservableCollection<StatItem>
    {
      new StatItem { Type = "Story Nodes", Count = 0 },
      new StatItem { Type = "Actors", Count = 0 },
      new StatItem { Type = "Items", Count = 0 },
      new StatItem { Type = "Skills", Count = 0 },
      new StatItem { Type = "Enemies", Count = 0 },
      new StatItem { Type = "Quests", Count = 0 }
    };

    PropertyChanged += (s, e) =>
    {
      if (e.PropertyName == nameof(StoryNodeCount))
        Statistics[0].Count = StoryNodeCount;
      else if (e.PropertyName == nameof(ActorCount))
        Statistics[1].Count = ActorCount;
      else if (e.PropertyName == nameof(ItemCount))
        Statistics[2].Count = ItemCount;
    };
  }

  // Resolution
  public ObservableCollection<string> Resolutions { get; } = new ObservableCollection<string>
  {
    "1920x1080",
    "1280x720",
    "1024x768"
  };

  // Window Mode
  public ObservableCollection<string> WindowModes { get; } = new ObservableCollection<string>
  {
    "Windowed",
    "Fullscreen",
    "Borderless"
  };

  // Starting Scene
  public ObservableCollection<string> StartingScenes { get; } = new ObservableCollection<string>
  {
    "MainMenu",
    "Intro",
    "Level1"
  };

  public ObservableCollection<StatItem> Statistics { get; }
}
public partial class StatItem : ObservableObject
{

  [ObservableProperty]
  int count;
  [ObservableProperty]
  string type = "";
}
