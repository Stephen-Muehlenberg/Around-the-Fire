using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
//    public HeroAction action;
    public bool actionPending;
  }

  [SerializeField] private CampUi ui;
  [SerializeField] private List<PortraitZoneUiGroup> zones;
  [SerializeField] private TimeOfDayController timeOfDayController;
  [SerializeField] private GameObject portraitPrefab;
  [SerializeField] private SelectOptionUI actionButtons;
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
    HeroAction defaultAction = null;// ActionManager.GetDefaultCampAction();

    Game.heroes.ForEach(hero =>
    {
      var info = new HeroCampInfo() { hero = hero };
      heroInfo.Add(hero, info);

      var heroUiObject = Instantiate(portraitPrefab);
      info.portrait = heroUiObject.GetComponent<Portrait>();
      MoveHeroTo(info, defaultZone);
      info.zone = defaultZone;

//      info.action = defaultAction;
      info.hero.action = defaultAction;
      info.portrait.Initialise(hero, Portrait.Interactions.CLICKABLE, this);
//      info.portrait.SetAction(info.action.titlePresentProgressive);
      info.portrait.SetAction(info.hero.action?.titlePresentProgressive);

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

  private void SelectHero(HeroCampInfo hero, bool showActions = true)
  {
    if (selectedHero != null)
      selectedHero.portrait.SetSelected(false);
    selectedHero = hero;
    selectedHero.portrait.SetSelected(true);
    ui.ShowStatsFor(hero.hero);
    if (showActions) ShowActionsFor(hero);
  }

  private void ShowActionsFor(HeroCampInfo hero)
  {
    var actions = ActionManager.GetCampActionsFor(hero.zone.zone);
    var options = actions.Select(it => it.ToOption(hero.hero, Game.state)).ToList();
    options.ForEach(it => UnityEngine.Debug.Log(it.title));
    var heroCopy = hero;

    actionButtons.Show(options, (option, index) => {
      var selectedAction = option.reference as HeroAction;
      heroCopy.portrait.SetAction(selectedAction.titlePresentProgressive);
//      heroCopy.action = selectedAction;
      heroCopy.hero.action = selectedAction;

      string message = selectedAction.GetAssignmentAnnouncement(heroCopy.hero, Game.state);
      SpeechBubble.Show(heroCopy.portrait.transform, message, null);

      actionButtons.Dismiss();
      OnActionSelected(heroCopy.hero);
    });
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
        action = new AC2T_Sleep();
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
      SelectHero(heroInfo[hero]);
      yield return SpeechBubble.Show(heroInfo[hero].portrait, "I feel like doing this.");
    }

    if (selectedHero != null) DeselectHero(selectedHero);
    uiState = UIState.INTERACTIVE;
  }

  public void OnActionSelected(Hero hero, bool wasAssignedBySelf = false)
  {
    if (hero.action != null && !wasAssignedBySelf)
    {
      string message = hero.action.GetAssignmentAnnouncement(hero, Game.state);
      SpeechBubble.Show(heroInfo[hero].portrait, message, null);
    }

    bool allHeroesReady = Game.heroes.All(it => it.action != null);
    confirmActionsButton.SetActive(allHeroesReady);
  }

  public void ConfirmActions()
  {
    // Disable action UI.
    uiState = UIState.UNINTERACTIVE;
    confirmActionsButton.SetActive(false);
    foreach (var (hero, heroCampInfo) in heroInfo)
    {
      //      heroCampInfo.portrait.AllowCancel(false);
      DeselectHero(heroCampInfo);
    }

    // Animate time advancing.
    timeOfDayController.AdvanceTime(
      startTime: (int) Game.time.hourOfDay,
      hours: 1,
      onHourEnd: null,
      onFinished: OnAdvanceTimeFinished);
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
    Game.party.AdjustHeroStatsOverTime(hoursPassed: hours);

    foreach (var (a, b) in heroInfo)
      b.actionPending = true;

    // Sort Heroes by their Action priority, then left-to-right, then top-to-bottom.
    heroInfo.Values.Where(it => it.actionPending)
      .OrderBy(it => it.portrait.transform.position.y)
      .OrderBy(it => it.portrait.transform.position.x)
//      .OrderBy(it => it.action.completionOrder)
      .OrderBy(it => it.hero.action.completionOrder)
      .ToList();

    ProcessNextAction();
  }

  /// <summary>
  /// Recursive method; keeps invoking itself until all <see cref="HeroCampInfo"/>s
  /// with pending actions are processed.
  /// </summary>
  private void ProcessNextAction()
  {
    foreach (var (_, heroCampInfo) in heroInfo)
      heroCampInfo.portrait.SetHighlighted(false);
//    ui.ShowStatsFor(selectedHero.hero);

    // If all actions have been completed, stop processing actions.
    if (!(heroInfo.Any(hero => hero.Value.actionPending)))
    {
      FinishActions();
      return;
    }

    var hero = heroInfo.Values.First(it => it.actionPending);
    hero.actionPending = false;

    hero.portrait.SetHighlighted(true);
    ui.ShowStatsFor(hero.hero);
    hero.portrait.SetAction(null);
    string message = hero.hero.action.GetCompletionAnnouncement(hero.hero, Game.state);
    SpeechBubble.Show(hero.portrait, message, () => {
      StartCoroutine(hero.hero.action.Process(hero.hero, previousGameState, Game.state, ProcessNextAction));
    });
  }

  private void FinishActions()
  {
    if (Game.heroes.All(it => it.action is AC2T_Sleep))
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

    // Clear hero's actions, unless they're sleeping.
    Game.heroes.ForEach(hero =>
    {
      if (hero.action is not AC2T_Sleep)
        hero.SelectAction(null);
    });

    // Update UI.
    ui.ShowPartyStats();
    uiState = UIState.INTERACTIVE;

    StartCoroutine(NewHourSequence(Game.time.hourOfDay));
  }

  public void OnEmptyBackgroundClick()
  {
    DeselectHero();
  }

  private void DeselectHero()
  {
    if (selectedHero != null)
    {
      selectedHero.portrait.SetSelected(false);
      selectedHero = null;
    }
    // TODO dismiss hero's UI, show party UI
    actionButtons.Dismiss();
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
    SelectHero(heroInfo[portrait.character as Hero], showActions: true);
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

  public void OnClickZone(PortraitZoneUiGroup zoneUi, PortraitZoneUiArea childClicked, PointerEventData data)
  {
    if (data.button == PointerEventData.InputButton.Left)
    {
      zoneUi.SetAppearance(PortraitZoneUiGroup.Appearance.HIDDEN);
      DeselectHero();
    }
    else if (data.button == PointerEventData.InputButton.Right)
    {
      zoneUi.SetAppearance(PortraitZoneUiGroup.Appearance.HIDDEN);
      MoveHeroTo(
        hero: selectedHero,
        zone: zoneUi,
        specificArea: childClicked,
        animateMove: true,
        showActions: true);
    }
  }

  private void MoveHeroTo(
    HeroCampInfo hero,
    PortraitZoneUiGroup zone,
    PortraitZoneUiArea specificArea = null,
    bool animateMove = false,
    bool showActions = false)
  {
    if (hero == null) return;

    if (zone.CanAccept(hero.portrait))
    {
      if (hero.zone != null)
        hero.zone.Remove(hero.portrait);
      zone.Add(hero.portrait, specificArea);
      hero.zone = zone;
      // TODO animate character moving/bouncing to location
      if (showActions) ShowActionsFor(hero);
    }
  }
}
