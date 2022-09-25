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

    if (Game.journey.fractionComplete >= 1)
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

  public void OnPortraitClick(Portrait portrait)
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
}
