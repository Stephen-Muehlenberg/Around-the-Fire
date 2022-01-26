using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TravelController : MonoBehaviour
{
  private const int AVG_HOURS_TRAVELLED_PER_DAY = 10;
  private const float REAL_SECONDS_PER_GAME_HOUR = 1f;

  public TMPro.TMP_Text timeOfDayText;
  public TMPro.TMP_Text journeyLengthText;
  public Slider progressSlider;
  public GameObject HaltButton, ContinueButton, RestButton, CampButton;
  public Transform portraitParent;
  public GameObject portraitPrefab;

  [SerializeField] private TravelBackground background;
  private List<HeroTravelBounce> heroBouncePortraits;

  private float hoursToDestination;
  private float hoursTravelled;
  private bool travelling;

  public void Start()
  {
    heroBouncePortraits = new List<HeroTravelBounce>(Party.heroes.Count);
    Party.heroes.ForEach(hero =>
    {
      var portraitObj = Instantiate(portraitPrefab, portraitParent);
      var portrait = portraitObj.GetComponent<HeroPortrait>();
      portrait.Initialise(hero, null, null);
      var bounce = portraitObj.AddComponent<HeroTravelBounce>();
      bounce.Initialize(hero);
      heroBouncePortraits.Add(bounce);
    });

    this.DoAfterDelay(0.3f, () => StartJourney(hoursToDestination: 30, hoursTravelled: 0));
  }

  private void Update()
  {
    if (!travelling) return;

    float hoursTravelledThisFrame = Time.deltaTime / REAL_SECONDS_PER_GAME_HOUR;
    hoursTravelled += hoursTravelledThisFrame;
    Party.currentState.time += hoursTravelledThisFrame;
    if (Party.time >= 24) Party.currentState.time -= 24;
    progressSlider.value = hoursTravelled / hoursToDestination;
    timeOfDayText.text = (Mathf.FloorToInt(Party.time) - (Party.time < 13 ? 0 : 12)) + (Party.time < 13 ? "am" : "pm");

    background.TravelDistance(hoursTravelledThisFrame);
  }

  public void StartJourney(float hoursToDestination, float hoursTravelled)
  {
    this.hoursToDestination = hoursToDestination;
    this.hoursTravelled = hoursTravelled;
    journeyLengthText.text = Mathf.CeilToInt(hoursToDestination / AVG_HOURS_TRAVELLED_PER_DAY) + " day journey";
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
}
