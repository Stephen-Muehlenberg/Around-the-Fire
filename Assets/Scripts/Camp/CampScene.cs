using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Logic for the Camp scene.
/// </summary>
public class CampScene : MonoBehaviour, PortraitZoneUiGroup.Interactions, Portrait.EventsCallback
{
  public enum UIState { INTERACTIVE, DRAG_IN_PROCESS, UNINTERACTIVE }

  /// <summary>All the scene-relevant info for a single hero, bundled together.</summary>
  private class HeroCampInfo
  {
    public Hero hero;
    public Portrait portrait;
    public PortraitZoneUiGroup zone;
    public BounceMovement bounceMovement;
    public HeroAction action;
    public bool actionPending;
  }

  [SerializeField] private CampUi ui;
  [SerializeField] private List<PortraitZoneUiGroup> zones;
  [SerializeField] private TimeOfDayController timeOfDayController;
  [SerializeField] private GameObject portraitPrefab;
  [SerializeField] private HeroStatsPanel heroStatsPanel;
  [SerializeField] private SelectOptionUI options;
  [SerializeField] private GameObject confirmActionsButton;

  private Dictionary<Hero, HeroCampInfo> heroInfo;
  private HeroCampInfo selectedHero;
  private GameState previousGameState;
  public UIState uiState = UIState.UNINTERACTIVE;

  private void Start()
  {
    // Initialise portrait zones.
    zones.ForEach(zoneUi =>
    {
      var zone = Camp.zones.First(zone => zoneUi.name == zone.name);
      zoneUi.Initialise(zone, this);
    });

    // Initialise hero portraits.
    heroInfo = new Dictionary<Hero, HeroCampInfo>(Game.heroes.Count);
    var defaultZone = zones.First(it => it.zone == Camp.zoneAround);
    var defaultAction = ActionManager.GetDefaultCampAction();

    Game.heroes.ForEach(hero =>
    {
      var info = new HeroCampInfo() { hero = hero };
      heroInfo.Add(hero, info);

      var heroUiObject = Instantiate(portraitPrefab);
      info.portrait = heroUiObject.GetComponent<Portrait>();
      defaultZone.Add(info.portrait);
      info.zone = defaultZone;

      info.action = defaultAction;
      info.portrait.Initialise(hero, Portrait.Interactions.CLICKABLE, this);
      info.portrait.SetAction(info.action.titlePresentProgressive);

      info.bounceMovement = heroUiObject.AddComponent<BounceMovement>();
      info.bounceMovement.Initialise(bounceHeight: 480f,
        bounceDuration: 1.35f,
        addSlightVarianceToBounce: true);
    });

    Game.state.camp = new Camp()
    {
      fire = Camp.FireState.NONE,
    };

    timeOfDayController.SetTime(Game.time.hourOfDay);
    FireEffects.SetState(Game.camp.fire);

  //  this.StartAfterDelay(0.5f, NewHourSequence(Game.time.hourOfDay));
  }

  private void SelectHero(Hero hero, bool showActions = true)
  {
    if (selectedHero != null)
      selectedHero.portrait.SetSelected(false);
    selectedHero = heroInfo[hero];
    selectedHero.portrait.SetSelected(true);
    heroStatsPanel.ShowStatsFor(hero);
    if (hero.location != null)
    {
  //    if (hero.action == null && showActions)
    //    hero.location.ShowActions(hero);
    //  else
      //  hero.location.ShowActions(null);
    }
  }

  private void DeselectHero(HeroCampInfo hero)
  {
    if (hero != null && hero.portrait != null)
      hero.portrait.SetSelected(false);
    if (selectedHero == hero)
      selectedHero = null;
  }

  private IEnumerator NewHourSequence(float hour)
  {
    yield return TimePopup.Show(hour);

    var heroesToProcess = new List<Hero>(Game.heroes.Count);
    Game.heroes.ForEach(it => heroesToProcess.Add(it));
    HeroAction action;
    float actionWeight;
    while (heroesToProcess.Count > 0)
    {
      // Choose a hero at random, so we don't always
      // have the first hero take the best available actions.
      int i = Random.Range(0, heroesToProcess.Count);
      var hero = heroesToProcess[i];
      heroesToProcess.RemoveAt(i);

      // If time is midnight, all heroes automatically sleep.
      if (Game.time.hourOfDay == 0)
        action = new ACT_Sleep();
      else
      {
        // Calculate the most desirable action for the hero.
        (action, actionWeight)
          = ActionManager.GetMostWantedCampAction(hero, Game.state);

        // Determine if Hero will assign themselves the task,
        // or resist the temptation.
        float randomVariance = Random.Range(0f, 1.25f);
        float outcome = (hero.mood / 100) + randomVariance - actionWeight;
        UnityEngine.Debug.Log(hero.name + " wants to " + action.title + " (mood " + (hero.mood / 100) + " + random " + randomVariance + " - weight " + actionWeight + " = " + outcome + ")");
        if (outcome > 0)
          continue;
      }

      // Assign task to self.
 //     yield return hero.portrait.AnimateMoveTo(action.location);
      hero.SelectAction(action, assignedBySelf: true);
      SelectHero(hero);
      yield return SpeechBubble.Show(hero.portrait, "I feel like doing this.");
    }

    if (selectedHero != null) DeselectHero(selectedHero);
    uiState = UIState.INTERACTIVE;
  }

  public void OnActionSelected(Hero hero, bool wasAssignedBySelf = false)
  {
    if (hero.action != null && !wasAssignedBySelf)
    {
      string message = hero.action.GetAssignmentAnnouncement(hero, Game.state);
      SpeechBubble.Show(hero.portrait, message, null);
    }

    bool allHeroesReady = Game.heroes.All(it => it.action != null);
    confirmActionsButton.SetActive(allHeroesReady);
  }

  public void ConfirmActions()
  {
    // Disable action UI.
    uiState = UIState.UNINTERACTIVE;
    confirmActionsButton.SetActive(false);
    // ActionList.Hide();
  //  Game.heroes.ForEach(it => {
    //  it.portrait.AllowCancel(false);
  //    DeselectHero(it);
//    });
    if (selectedHero != null)
      DeselectHero(selectedHero);

    // Animate time advancing.
    timeOfDayController.AdvanceTime((int) Game.time.hourOfDay, 1, null, OnAdvanceTimeFinished);
  }

  private void OnAdvanceTimeFinished(float timePassed, float newTime)
  {
    // Update UI.
    Game.time.Advance(timePassed);

    // Copy current party & camp state, so that any changes made as a
    // result of one Action don't affect calculations of subsequent Actions.
    previousGameState = Game.state.DeepCopy();

    // Degrade Hero stats for every hour passed.
    int hours = Game.heroes
      .Select(it => it.action.hours)
      .Min();
    DegradeStatsOverTime(hours);

    // Sort Heroes by their Action priority, then left-to-right, then top-to-bottom.
    heroInfo.Values.Where(it => it.actionPending)
      .OrderBy(it => it.portrait.transform.position.y)
      .OrderBy(it => it.portrait.transform.position.x)
      .OrderBy(it => it.action.completionOrder)
      .ToList();

    ProcessNextAction();
  }

  private void DegradeStatsOverTime(int hours)
  {
    Game.heroes.ForEach(it => {
      for (int i = 0; i < hours; i++)
      {
        it.hunger -= Random.Range(3.5f, 4.5f);
        it.rest -= Random.Range(4.5f, 5.5f);

        // Mood tends towards the average of the other three stats.
        float statAverage = (it.health + it.hunger + it.rest) / 3f;
        float moodOffset = statAverage - it.mood;
        // Rough output: 0: 0,  5: 1.8,  20: 3.5,  50: 4.7,  100: 5.7
        float moodDelta = Mathf.Log((Mathf.Abs(moodOffset) / 2) + 1, 2);
        if (moodOffset < 0) moodDelta = -moodDelta;

        // Other stats can only improve mood so much.
        else if (moodOffset > 0)
        {
          if (it.mood + moodDelta > 65) moodDelta = 0;
          if (it.mood + moodDelta > 55) moodDelta /= 2;
        }

        it.mood += moodDelta + Random.Range(-0.5f, 0.5f);
      }
    });
  }

  /// <summary>
  /// Recursive method; keeps invoking itself until all <see cref="HeroCampInfo"/>s
  /// with pending actions are processed.
  /// </summary>
  private void ProcessNextAction()
  {
 /*   Game.heroes.ForEach(it => it.portrait.Unhighlight());
    heroStatsPanel.ShowStatsFor(selectedHero);
    if (heroInfo.Any(it => it.Value.actionPending))
    {
      FinishActions();
      return;
    }

    var hero = heroInfo.Values.First(it => it.actionPending);
    hero.actionPending = false;

    hero.portrait.Highlight();
    heroStatsPanel.ShowStatsFor(hero.hero);
    hero.portrait.ClearActionText();
    string message = hero.action.GetCompletionAnnouncement(hero, Game.state);
    SpeechBubble.Show(hero.portrait, message, () => {
      StartCoroutine(hero.action.Process(hero.hero, previousGameState, Game.state, ProcessNextAction));
    });*/
  }

  private void FinishActions()
  {
    if (Game.heroes.All(it => it.action is ACT_Sleep))
    {
      float timeUntil8 = Game.time.hourOfDay < 8
        ? 8 - Game.time.hourOfDay
        : 32 - Game.time.hourOfDay;
      Game.time.Advance(timeUntil8, updateRest: false);
      Game.heroes.ForEach(it => {
        it.hoursAwake = 0;
        it.rest += (int) timeUntil8 * 10;
      });
      UnityEngine.SceneManagement.SceneManager.LoadScene("Travel");
      return;
    }

    // Clear hero's actions.
    Game.heroes.ForEach(it => it.SelectAction(null));

    // Update UI.
    heroStatsPanel.ShowStatsFor(null);
    uiState = UIState.INTERACTIVE;

    StartCoroutine(NewHourSequence(Game.time.hourOfDay));
  }

  public void OnPointerEnterPortrait(Portrait portrait)
  {
    // TODO show highlight character stats
  }

  public void OnPointerExitPortrait(Portrait portrait)
  {
    // TODO show highlighted character stats
  }

  public void OnPortraitLeftClick(Portrait portrait)
  {
    if (selectedHero != null)
      selectedHero.portrait.SetSelected(false);
    selectedHero = heroInfo[portrait.character as Hero];
    selectedHero.portrait.SetSelected(true);
  }

  public void OnPointerEnterZone(PortraitZoneUiGroup zoneUi, PortraitZoneUiArea childEntered)
  {
    if (selectedHero == null) return;

    if (zoneUi.CanAccept(selectedHero.portrait))
      zoneUi.SetAppearance(PortraitZoneUiGroup.Appearance.VALID_TARGET);
    else
      zoneUi.SetAppearance(PortraitZoneUiGroup.Appearance.INVALID_TARGET);
  }

  public void OnPointerExitZone(PortraitZoneUiGroup zoneUi, PortraitZoneUiArea childExited)
  {
    zoneUi.SetAppearance(PortraitZoneUiGroup.Appearance.HIDDEN);
  }

  public void OnRightClickZone(PortraitZoneUiGroup zoneUi, PortraitZoneUiArea childClicked)
  {
    if (selectedHero == null) return;

    if (zoneUi.CanAccept(selectedHero.portrait))
    {
      selectedHero.zone.Remove(selectedHero.portrait);
      zoneUi.Add(selectedHero.portrait);
      zoneUi.SetAppearance(PortraitZoneUiGroup.Appearance.HIDDEN);
      // TODO show new location's actions
      // TODO animate character moving/bouncing to location
    }
  }
}
