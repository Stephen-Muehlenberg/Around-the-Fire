using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Logic for the Travel scene.
/// </summary>
public class TravelScene : MonoBehaviour, Portrait.EventsCallback
{
  /// <summary>All the scene-relevant info for a single hero, bundled together.</summary>
  private class HeroTravelInfo
  {
    public Hero hero;
    public Portrait portrait;
    public BounceMovement bounceMovement;
    public HeroAction action;
  }

  /// <summary>Note: State.NONE corresponds to the initial state which only exists for
  /// a fraction of a second while stuff is being initialised. Most things should
  /// not be interactive during this time.</summary>
  public enum TravelState { NONE, TRAVELLING, RESTING, PAUSED, ARRIVED }

  private const float BASE_KM_PER_HOUR = 4;
  private const float REAL_SECONDS_PER_GAME_HOUR = 2f;

  public TravelUi ui;
  public Transform portraitParent;
  public GameObject portraitPrefab;

  [SerializeField] private HeroStatsPanel heroStatsPanel;
  [SerializeField] private EncounterPanel encounterPanel;
  [SerializeField] private TravelBackground background;
  [SerializeField] private TimeOfDayController timeOfDay;
  [SerializeField] private SelectOptionUI taskButtons;
  private Dictionary<Hero, HeroTravelInfo> heroInfo;
  private Portrait selectedPortrait;

  private TravelState travelState = TravelState.NONE;

  // Cached to avoid frequent reallocation.
  private float hoursPassedThisFrame;
  private int previousHour;
  private float kilometersTravelledThisFrame;

  public void Start()
  {
    heroInfo = new Dictionary<Hero, HeroTravelInfo>(Game.heroes.Count);

    Game.heroes.ForEach(hero =>
    {
      var info = new HeroTravelInfo() { hero = hero };
      heroInfo.Add(hero, info);

      // Have heroes assign themselves an action.
      var (autoSelectedAction, weight) = ActionManager.GetMostWantedTravelAction(hero, Game.state);
      info.action = autoSelectedAction;

      // Display heroes as bouncing portraits.
      var heroUiObject = Instantiate(portraitPrefab, portraitParent);
      info.portrait = heroUiObject.GetComponent<Portrait>();
      info.bounceMovement = heroUiObject.AddComponent<BounceMovement>();

      info.portrait.Initialise(hero, Portrait.Interactions.CLICKABLE, this);
      info.portrait.SetAction(info.action.titlePresentProgressive);
      info.bounceMovement.Initialise(bounceHeight: 480f,
        bounceDuration: 1.35f,
        addSlightVarianceToBounce: true);

      // Bit of a hack. Normally, the Bounce scripts would initialise
      // their starting positions BEFORE the HorizontalLayoutScript
      // aligns them. So as soon as they started bouncing, they would
      // revert to their initial position.
      // By adding a slight delay, this fixes the issue, while also
      // preventing the characters from all bouncing in eerily perfect sync.
      float randomStartDelay = Random.Range(0.1f, 0.3f);
      this.DoAfterDelay(randomStartDelay, () => {
        info.bounceMovement.SetGroundPointToCurrentPosition();
        info.bounceMovement.StartBouncing();
      });
    });

    ui.Initialise(
      day: Game.journey.dayOfTravel,
      timeOfDay: Game.time.timeOfDayDescription,
      destination: Game.journey.destination.name,
      estimatedJourneyLengthDays: Game.journey.estimatedDurationDays);
    timeOfDay.SetTime(Game.time.hourOfDay);

    travelState = TravelState.PAUSED;
    this.DoAfterDelay(0.3f, () => SetState(TravelState.TRAVELLING));
  }

  private void Update()
  {
    if (travelState != TravelState.TRAVELLING && travelState != TravelState.RESTING) return;

    // Update time of day and scene lighting.
    hoursPassedThisFrame = Time.deltaTime / REAL_SECONDS_PER_GAME_HOUR;
    if (travelState == TravelState.RESTING)
      hoursPassedThisFrame *= 2;
    previousHour = (int) Game.time.hourOfDay;
    Game.time.Advance(hoursPassedThisFrame);
    ui.SetTime(
      day: Game.journey.dayOfTravel,
      timeOfDay: Game.time.timeOfDayDescription);
    timeOfDay.SetTime(Game.time.hourOfDay);

    // Calculate distance travelled.
    float speedFraction = travelState == TravelState.TRAVELLING
      ? GetSpeedFraction()
      : 0;
    kilometersTravelledThisFrame = BASE_KM_PER_HOUR * speedFraction * hoursPassedThisFrame;
    Game.journey.kilometresTravelled += kilometersTravelledThisFrame;

    // Update UI and parallax background.
    if (travelState == TravelState.TRAVELLING)
      ShowSpeedText(speedFraction);
    background.TravelDistance(kilometersTravelledThisFrame);

    // Update party stats on the hour mark.
    // TODO invoke hero actions on each update, instead of on the hour mark.
    if (previousHour != (int) Game.time.hourOfDay)
    {
      Game.heroes.ForEach(it => it.UpdateStatsAtEndOfHour());
    }


    // Update UI to reflect party and journey state.
    DisplaySpeedModifiers();

    if (Game.journey.fractionComplete >= 1 && travelState == TravelState.TRAVELLING)
      SetState(TravelState.ARRIVED);
  }

  /// <summary>
  /// Returns the party's current speed, as a fraction of the default
  /// speed. Takes into account the party's stats, terrain, and time of day.
  /// May exceed 100%.
  /// </summary>
  private float GetSpeedFraction()
  {
    // Assumptions: IRL travellers can maintain 4km/h for 8 hours a day.
    // This can go up to 5km/h in good conditions, and much lower in bad conditions.
    float speedPercent = 100;

    float[] lowestStats = new float[] {
      Game.heroes.Select(it => it.health).Min(),
      Game.heroes.Select(it => it.hunger).Min(),
      Game.heroes.Select(it => it.mood).Min(),
      Game.heroes.Select(it => it.rest).Min(),
    };

    foreach (float stat in lowestStats)
      speedPercent += stat switch
      {
        > 90 => +10,
        < 50 and >= 25 => -10,
        < 25 and >= 5 => -20,
        < 5 => -40,
        _ => 0
      };

    if (!Game.time.isDaytime) speedPercent -= 20;

    // TODO take into account terrain.

    return Mathf.Max(speedPercent, 0) / 100f;
  }

  private void ShowSpeedText(float speedFraction)
  {
    string speedDescription = speedFraction switch
    {
      > 1 => "Quickly",
      > 0.8f and <= 1 => "Steadily",
      > 0.6f and <= 0.8f => "Slowly",
      _ => "Very Slowly"
    };
    ui.SetSpeed(speedDescription);
  }

  private void SetState(TravelState state)
  {
    this.travelState = state;
    ui.SetState(state);
    SetPortraitsBouncing(state == TravelState.TRAVELLING);
  }

  /// <summary>Tells portraits to stop bouncing next time they hit the ground.</summary>
  public void SetPortraitsBouncing(bool bouncing)
  {
    foreach (KeyValuePair<Hero, HeroTravelInfo> it in heroInfo)
    {
      if (bouncing)
        it.Value.bounceMovement.StartBouncing();
      else
        it.Value.bounceMovement.StopBouncing();
    }
  }

  public void Rest() => SetState(TravelState.RESTING);

  public void Camp()
  {
    SceneManager.LoadSceneAsync("Camp");
  }

  public void OnPointerEnterPortrait(Portrait portrait)
  {
    heroStatsPanel.ShowStatsFor(portrait.character as Hero);
  }

  public void OnPointerExitPortrait(Portrait portrait)
  {
    if (selectedPortrait == null)
      heroStatsPanel.ShowStatsFor(null);
    else
      heroStatsPanel.ShowStatsFor(selectedPortrait.character as Hero);
  }

  public void OnPortraitLeftClick(Portrait portrait)
  {
    // Change selected hero.
    if (selectedPortrait != null)
      selectedPortrait.SetSelected(false);
    portrait.SetSelected(true);
    selectedPortrait = portrait;

    // Show available actions.
    var actions = ActionManager.GetTravelActionsFor(portrait.character as Hero, Game.state);
    var options = actions.Select(it => it.ToOption()).ToList();
    taskButtons.Show(options, (option, index) => {
      if (selectedPortrait == null)
        throw new System.Exception("Clicked on an action, but there's no hero selected!");

      var hero = selectedPortrait.character as Hero;
      var selectedAction = option.reference as HeroAction;
      selectedPortrait.SetAction(selectedAction.titlePresentProgressive);
      heroInfo[hero].action = selectedAction;

      string message = selectedAction.GetAssignmentAnnouncement(hero, Game.state);
      SpeechBubble.Show(selectedPortrait.transform, message, null);
      // TODO have hero bounce to position.

      taskButtons.Dismiss();
    });
  }

  public void OnEmptyBackgroundClick()
  {
    if (selectedPortrait != null)
      selectedPortrait.SetSelected(false);
    taskButtons.Dismiss();
  }

  public void OnHaltResumeClick()
  {
    if (travelState == TravelState.PAUSED)
      SetState(TravelState.TRAVELLING);
    // If already arrived and just resting, return to Arrived state.
    else if (travelState == TravelState.RESTING
        && Game.journey.fractionComplete >= 1)
      SetState(TravelState.ARRIVED);
    else
      SetState(TravelState.PAUSED);
  }

  public void LoadDestinationScene()
  {
    // TODO Replace this super placeholder logic.
    if (Game.journey.destination.isTown)
    {
      Game.state.journey = null;
      SceneManager.LoadScene("Town");
    } else
    {
      Game.party.quest.complete = true;
      Game.state.journey = new Journey()
      {
        destination = new Location()
        {
          name = "Nice Town",
          isTown = true
        },
        distanceKm = 10,
        startTime = Game.time.Copy(),
      };
      SceneManager.LoadScene("Travel");
    }
  }



















  /*
   * Each frame, get each character's stats.
   * Convert their stats into tiers.
   * For each stat type, get the number of players on the lowest tier.
   * If they all match last frame's counts, 
   */

  // Cached for re-use each frame.
  private int i;
  private (int, int)[] lowestStatsAndCount = new (int, int)[4];
  private int statTier;
  private List<TravelUi.SpeedModifier> modifierUiInfo = new List<TravelUi.SpeedModifier>(5); // TODO increase size if/when we get more possible modifiers.
  private TravelUi.SpeedModifier[] statModifiers = new TravelUi.SpeedModifier[4]
  {
    new TravelUi.SpeedModifier(),
    new TravelUi.SpeedModifier(),
    new TravelUi.SpeedModifier(),
    new TravelUi.SpeedModifier(),
  };
  private TravelUi.SpeedModifier statModifier;

  /// <summary>
  /// Calculate all the speed modifiers currently being applied, and display them.
  /// </summary>
  private void DisplaySpeedModifiers()
  {
    // Convert hero stats to to tiers. Find the lowest tier for each stat, and the count.
    for (i = 0; i < 4; i++)
      lowestStatsAndCount[i] = (int.MaxValue, 0);
    for (i = Game.heroes.Count - 1; i >= 0; i--)
    {
      CompareStats(Game.heroes[i].health, ref lowestStatsAndCount[0]);
      CompareStats(Game.heroes[i].rest, ref lowestStatsAndCount[1]);
      CompareStats(Game.heroes[i].hunger, ref lowestStatsAndCount[2]);
      CompareStats(Game.heroes[i].mood, ref lowestStatsAndCount[3]);
    }

    modifierUiInfo.Clear();
    if (!Game.time.isDaytime)
      modifierUiInfo.Add(new TravelUi.SpeedModifier() { descritption = "Night Time", up = false });
    // TODO:
    // Bad weather
    // Difficult terrain

    // Convert stat info into UI info.
    for (i = 0; i < 4; i++)
    {
      var bb = lowestStatsAndCount[i];
      if (bb.Item1 == 2) continue; // No modifiers if stat is at tier 2 (normal).
      var aa = statModifiersByTier[i];
      var cc = aa[bb.Item1];
      statModifier = statModifiers[i];
      statModifier.up = bb.Item1 > 2;
      statModifier.descritption = cc.description;
      statModifier.count = bb.Item2;
      modifierUiInfo.Add(statModifier);
    }

    // Display stat info.
    ui.SetSpeedModifiers(modifierUiInfo);
  }

  // TODO Move this elsewhere. It probably belongs in its own class? It's more closely
  // associate with heroes and stats than travel in particular.
  private readonly StatTier[][] statModifiersByTier = new StatTier[4][]
  {
    new StatTier[4]
    {
      new StatTier("Severe Injury", -35),
      new StatTier("Injury", -15),
      new StatTier("Fair Health", 0),
      new StatTier("Excellent Health", 5),
    },
    new StatTier[4]
    {
      new StatTier("Exhausted", -25),
      new StatTier("Tired", -10),
      new StatTier("Adequately Rested", 0),
      new StatTier("Well Rested", 10),
    },
    new StatTier[4]
    {
      new StatTier("Starving", -20),
      new StatTier("Hungry", -5),
      new StatTier("Sated", 0),
      new StatTier("Well Fed", 5),
    },
    new StatTier[4]
    {
      new StatTier("Awful Morale", -20),
      new StatTier("Poor Morale", -5),
      new StatTier("Fair Mood", 0),
      new StatTier("High Spirits", 5),
    },
  };

  /// <summary>
  /// Rules about a hero stat (health, hunger, stamina, morale) when at a given
  /// tier (25% increments).
  /// </summary>
  private class StatTier {
    public string description;
    public int speedPercentModifier;

    public StatTier(string description, int speedPercentModifier)
    {
      this.description = description;
      this.speedPercentModifier = speedPercentModifier;
    }
  }

  private void CompareStats(float stat, ref (int, int) lowestTierAndCount)
  {
    statTier = Mathf.Clamp(Mathf.FloorToInt(stat / 25), 0, 3);
    if (statTier < lowestTierAndCount.Item1)
    {
      lowestTierAndCount.Item1 = statTier;
      lowestTierAndCount.Item2 = 1;
    } else if (statTier == lowestTierAndCount.Item1)
      lowestTierAndCount.Item2++;
  }
}
