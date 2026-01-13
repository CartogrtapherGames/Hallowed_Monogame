using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HallowedEditor.ViewModels;

public partial class SystemViewModel : ObservableObject
{
    [ObservableProperty]
    private string projectName = "My Game Project";
    
    [ObservableProperty]
    private string version = "1.0.0";
    
    [ObservableProperty]
    private string author = "";
    
    [ObservableProperty]
    private string description = "";
    
    // Resolution
    public ObservableCollection<string> Resolutions { get; } = new()
    {
        "1920x1080",
        "1280x720",
        "1024x768"
    };
    
    [ObservableProperty]
    private string selectedResolution;
    
    // Window Mode
    public ObservableCollection<string> WindowModes { get; } = new()
    {
        "Windowed",
        "Fullscreen",
        "Borderless"
    };
    
    [ObservableProperty]
    private string selectedWindowMode;
    
    // Starting Scene
    public ObservableCollection<string> StartingScenes { get; } = new()
    {
        "MainMenu",
        "Intro",
        "Level1"
    };
    
    [ObservableProperty]
    private string selectedStartingScene;
    
    [ObservableProperty]
    private string currency = "Gold";
    
    [ObservableProperty]
    private int maxPartySize = 4;
    
    [ObservableProperty]
    private bool vSyncEnabled = true;
    
    // Game Options
    [ObservableProperty]
    private bool enableAutosave = true;
    
    [ObservableProperty]
    private bool showMessageSkip = true;
    
    [ObservableProperty]
    private bool enableCombat = true;
    
    [ObservableProperty]
    private bool skipTitleScreen = true;
    
    [ObservableProperty]
    private bool enableInventory = true;
    
    [ObservableProperty]
    private bool enableQuests = true;
    
    [ObservableProperty]
    private bool enableSkills = true;
    
    [ObservableProperty]
    private bool enableEquipment = true;
    
    // Statistics
    [ObservableProperty]
    private int storyNodeCount = 0;
    
    [ObservableProperty]
    private int actorCount = 0;
    
    [ObservableProperty]
    private int itemCount = 0;
    
    public ObservableCollection<StatItem> Statistics { get; }
    
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
}

public partial class StatItem : ObservableObject
{
    [ObservableProperty]
    private string type = "";
    
    [ObservableProperty]
    private int count;
}