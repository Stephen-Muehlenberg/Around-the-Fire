using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

  private const float BASE_KM_PER_HOUR = 4;
  private const float REAL_SECONDS_PER_GAME_HOUR = 2f;

  public TMPro.TMP_Text journeyLengthText;
  public Slider progressSlider;
  public GameObject HaltButton, ContinueButton, RestButton, CampButton;
  public Transform portraitParent;
  public GameObject portraitPrefab2;

  [SerializeField] private HeroStatsPanel heroStatsPanel;
  [SerializeField] private TravelBackground background;
  [SerializeField] private TimeOfDayController timeOfDay;
  [SerializeField] private SelectOptionUI taskButtons;
  private Dictionary<Hero, HeroTravelInfo> heroInfo;
  private Portrait selectedPortrait;

  private bool travelling;

  // Cached to avoid frequent reallocation.
  private float hoursTravelledThisFrame;
  private int previousHour;
  private float kilometersTravelledThisFrame;

  public void Start()
  {
    ActionManager.Initialise(); // Temp; should be initialised elsewhere.

    heroInfo = new Dictionary<Hero, HeroTravelInfo>(Game.heroes.Count);

    Game.heroes.ForEach(hero =>
    {
      var info = new HeroTravelInfo() { hero = hero };
      heroInfo.Add(hero, info);

      // Have heroes assign themselves an action.
      var (autoSelectedAction, weight) = ActionManager.GetMostWantedTravelAction(hero, Game.state);
      info.action = autoSelectedAction;

      // Display heroes as bouncing portraits.
      var heroUiObject = Instantiate(portraitPrefab2, portraitParent);
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

    timeOfDay.SetTime(Game.time.hourOfDay);

    this.DoAfterDelay(0.3f, () => StartJourney(Game.party.journey));
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

  private void Update()
  {
    if (!travelling) return;

    // Update time of day and scene lighting.
    hoursTravelledThisFrame = Time.deltaTime / REAL_SECONDS_PER_GAME_HOUR;
    Game.party.journey.hoursTravelled += hoursTravelledThisFrame;
    previousHour = (int) Game.time.hourOfDay;
    Game.time.Advance(hoursTravelledThisFrame);
    timeOfDay.SetTime(Game.time.hourOfDay);

    // Calculate distance travelled.
    float speedFraction = GetSpeedFraction();
    kilometersTravelledThisFrame = BASE_KM_PER_HOUR * speedFraction * hoursTravelledThisFrame;
    Game.party.journey.kilometresTravelled += kilometersTravelledThisFrame;

    // Update UI and parallax background.
    ShowSpeedText(speedFraction);
    progressSlider.value = Game.party.journey.fractionComplete;
    background.TravelDistance(kilometersTravelledThisFrame);

    // Update party stats on the hour mark.
    if (previousHour != (int) Game.time.hourOfDay)
    {
      Game.heroes.ForEach(it => it.UpdateStatsAtEndOfHour());
    }

    // TODO invoke hero actions on each update

    if (Game.party.journey.fractionComplete >= 1)
      ReachDestination();
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
    journeyLengthText.text = Mathf.CeilToInt(Game.party.journey.estimatedDurationDays) + " day journey"
      + "\nTravelling " + speedDescription;
  }

  public void StartJourney(Journey journey)
  {
    journeyLengthText.text = Mathf.CeilToInt(journey.estimatedDurationDays) + " day journey";
    Continue();
  }

  private void ReachDestination()
  {
    Halt();
    return;

    // TODO Replace this super placeholder logic.
    if (Game.party.journey.townIsDestination)
    {
      Game.party.journey = null;
      SceneManager.LoadScene("Town");
    }
    else
    {
      Game.party.quest.complete = true;
      Game.party.journey = new Journey()
      {
        townIsDestination = true,
        distanceKm = 10
      };
      SceneManager.LoadScene("Travel");
    }
  }

  public void Halt()
  {
    HaltButton.SetActive(false);
    ContinueButton.SetActive(true);
    RestButton.SetActive(true);
    CampButton.SetActive(true);
    foreach (KeyValuePair<Hero, HeroTravelInfo> it in heroInfo)
      it.Value.bounceMovement.StopBouncing();

    travelling = false;
  }

  public void Continue()
  {
//    HaltButton.SetActive(true);
//    ContinueButton.SetActive(false);
//    RestButton.SetActive(false);
//    CampButton.SetActive(false);
//    heroBouncePortraits.ForEach(it => it.StartBouncing());
    travelling = true;
  }

  public void Rest()
  {

  }

  public void Camp()
  {
    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Camp");
  }

  public void OnPortraitPointerEnter(Portrait portrait)
  {
    heroStatsPanel.ShowStatsFor(portrait.character as Hero);
  }

  public void OnPortraitPointerExit(Portrait portrait)
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
}
