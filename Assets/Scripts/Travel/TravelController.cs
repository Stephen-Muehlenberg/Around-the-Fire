using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TravelController : MonoBehaviour, HeroPortrait.EventsCallback
{
  private const float BASE_KM_PER_HOUR = 4;
  private const float REAL_SECONDS_PER_GAME_HOUR = 2f;

  public TMPro.TMP_Text journeyLengthText;
  public Slider progressSlider;
  public GameObject HaltButton, ContinueButton, RestButton, CampButton;
  public Transform portraitParent;
  public GameObject portraitPrefab;
  public GameObject portraitPrefab2;

  [SerializeField] private HeroStatsPanel heroStatsPanel;
  [SerializeField] private TravelBackground background;
  [SerializeField] private TimeOfDayController timeOfDay;
  [SerializeField] private SelectOptionUI taskButtons;
  private List<HeroTravelBounce> heroBouncePortraits;
  private Dictionary<Hero, HeroAction> heroActions;

  private bool travelling;

  // Cached to avoid frequent reallocation.
  private float hoursTravelledThisFrame;
  private int previousHour;
  private float kilometersTravelledThisFrame;

  public void Start()
  {
    ActionManager.Initialise(); // Temp; should be initialised elsewhere.

    // Have heroes assign themselves an action.
    heroActions = new Dictionary<Hero, HeroAction>(Party.heroes.Count);
    Party.heroes.ForEach(hero =>
    {
      var (autoSelectedAction, weight) = ActionManager.GetMostWantedTravelAction(hero, Party.currentState);
      heroActions.Add(hero, autoSelectedAction);
    });

    // Display heroes as bouncing portraits.
    Party.heroes.ForEach(hero =>
    {
      var portraitObj = Instantiate(portraitPrefab2, portraitParent);
      var portrait = portraitObj.GetComponent<Portrait>();
      portrait.Initialise(hero, Portrait.Interactions.CLICKABLE);
      portrait.SetAction(heroActions[hero].titlePresentProgressive);

      var bounce = portraitObj.AddComponent<BounceMovement>();
      bounce.Initialise(addSlightVarianceToBounce: true); // TODO Set bounce height and time.
      float randomStartDelay = Random.Range(0.1f, 0.3f);
      // Bit of a hack. Normally, the Bounce scripts would initialise
      // their starting positions BEFORE the HorizontalLayoutScript
      // aligns them. So as soon as they started bouncing, they would
      // revert to their initial position.
      // By adding a slight delay, this fixes the issue, while also
      // preventing the characters from all bouncing in eerily perfect sync.
      this.DoAfterDelay(randomStartDelay, () => {
        bounce.SetGroundPointToCurrentPosition();
        bounce.StartBouncing();
      });
    });


    /*
    heroBouncePortraits = new List<HeroTravelBounce>(Party.heroes.Count);
    Party.heroes.ForEach(hero =>
    {
      var portraitObj = Instantiate(portraitPrefab, portraitParent);
      var portrait = portraitObj.GetComponent<HeroPortrait>();
      portrait.Initialise(hero, null, this);

      var bounce = portraitObj.AddComponent<HeroTravelBounce>();
      bounce.Initialize(hero);
      heroBouncePortraits.Add(bounce);
    });
    timeOfDay.SetTime(Party.timeOfDay);

    this.DoAfterDelay(0.3f, () => StartJourney(Party.journey));
    */

    timeOfDay.SetTime(Party.timeOfDay);
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
      Party.heroes.Select(it => it.health).Min(),
      Party.heroes.Select(it => it.hunger).Min(),
      Party.heroes.Select(it => it.mood).Min(),
      Party.heroes.Select(it => it.rest).Min(),
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

    if (!Party.isDaytime) speedPercent -= 20;

    // TODO take into account terrain.

    return Mathf.Max(speedPercent, 0) / 100f;
  }

  private void Update()
  {
    if (!travelling) return;

    // Update time of day and scene lighting.
    hoursTravelledThisFrame = Time.deltaTime / REAL_SECONDS_PER_GAME_HOUR;
    Party.journey.hoursTravelled += hoursTravelledThisFrame;
    previousHour = (int) Party.timeOfDay;
    Party.AdvanceTime(hoursTravelledThisFrame);
    timeOfDay.SetTime(Party.timeOfDay);

    // Calculate distance travelled.
    float speedFraction = GetSpeedFraction();
    kilometersTravelledThisFrame = BASE_KM_PER_HOUR * speedFraction * hoursTravelledThisFrame;
    Party.journey.kilometresTravelled += kilometersTravelledThisFrame;
    ShowSpeedText(speedFraction);

    // Update progress slider and parallax background.
    progressSlider.value = Party.journey.fractionComplete;
    background.TravelDistance(kilometersTravelledThisFrame);

    // Update party stats on the hour mark.
    if (previousHour != (int) Party.timeOfDay)
    {
      Party.heroes.ForEach(it => it.UpdateStatsAtEndOfHour());
    }
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
    journeyLengthText.text = Mathf.CeilToInt(Party.journey.estimatedDurationInDays) + " day journey"
      + "\nTravelling " + speedDescription;
  }

  public void StartJourney(JourneyState journey)
  {
    journeyLengthText.text = Mathf.CeilToInt(journey.estimatedDurationInDays) + " day journey";
    Continue();
  }

  public void Halt()
  {
    HaltButton.SetActive(false);
    ContinueButton.SetActive(true);
    RestButton.SetActive(true);
    CampButton.SetActive(true);
    heroBouncePortraits.ForEach(it => it.StopBouncing());
    travelling = false;
  }

  public void Continue()
  {
    HaltButton.SetActive(true);
    ContinueButton.SetActive(false);
    RestButton.SetActive(false);
    CampButton.SetActive(false);
    heroBouncePortraits.ForEach(it => it.StartBouncing());
    travelling = true;
  }

  public void Rest()
  {

  }

  public void Camp()
  {
    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Camp");
  }

  public void OnPointerEnterPortrait(HeroPortrait portrait)
  {
  //  if (uiState != UIState.INTERACTIVE) return;
    portrait.Highlight();
    heroStatsPanel.ShowStatsFor(portrait.hero);
  }

  public void OnPointerExitPortrait(HeroPortrait portrait)
  {
  //  if (uiState != UIState.INTERACTIVE) return;
    portrait.Unhighlight();
  //  HeroStatsPanel.ShowStatsFor(selectedHero);
  }

  // Unused portrait interactions.
  public void OnPointerClickPortrait(HeroPortrait portrait) {}
  public void OnPortraitDragStart(HeroPortrait portrait, PointerEventData data) {}
  public void OnPotraitDrag(HeroPortrait portrait, PointerEventData data) {}
  public void OnPotraitDragEnd(HeroPortrait portrait, PointerEventData data) {}
  public void OnPortaitCancelPressed(HeroPortrait portrait) {}
}
