using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Logic for the Camp scene.
/// </summary>
public class CampScene : MonoBehaviour, Portrait.EventsCallback
{
  public enum UIState { INTERACTIVE, DRAG_IN_PROCESS, UNINTERACTIVE }
  public enum Location { Around, Fire, Tent, Supplies, Clearing, Forest }

  /// <summary>All the camp-scene-relevant info for a single hero, bundled together.</summary>
  public class HeroCampInfo
  {
    public Hero hero;
    public HeroPortrait portrait;
    public Location location;
    public bool awake = true;
    //TODO    public BounceMovement bounceMovement;
    public HeroAction action;
    public HeroAction selfAssignedAction;
    public bool actionPending;
  }

  [SerializeField] private CampUi ui;
  [SerializeField] private List<LayoutGroup> locationLayoutGroups;
  [SerializeField] private TimeOfDayController timeOfDayController;
  [SerializeField] private GameObject portraitPrefab;
  [SerializeField] private SelectOptionUI actionButtons;
  [SerializeField] private GameObject confirmActionsButton;

  private List<HeroCampInfo> heroes;
  private HeroCampInfo selectedHero;
  private GameState previousGameState;
  private readonly CampActionManager actionManager = new();
  public UIState uiState = UIState.UNINTERACTIVE;

  private void Start()
  {
    // Initialise camp & scene.
    timeOfDayController.SetTime(Game.time.hourOfDay);
    Game.state.camp = new Camp() { fire = Camp.FireState.NONE };
    FireEffects.SetState(Game.camp.fire);

    // Initialise heroes.
    heroes = new(Game.heroes.Count);
    Game.heroes.ForEach(hero =>
    {
      var portrait = Instantiate(portraitPrefab).GetComponent<HeroPortrait>();
      portrait.Initialise(hero, Portrait.Interactions.CLICKABLE, this);
      portrait.SetAction(null);

      var info = new HeroCampInfo() {
        hero = hero,
        portrait = portrait,
        location = Location.Around,
        action = null,
        selfAssignedAction = null,
        actionPending = false
      };

      heroes.Add(info);
      SetHeroLocation(info, Location.Around);
    });

    // Start the main camp loop.
    this.StartAfterDelay(0.5f, AnimateToNextHour(Game.time.hourOfDay, false));
  }

  /// <summary>
  /// Starts a new hour sequence, 
  /// </summary>
  private IEnumerator AnimateToNextHour(float hour, bool animateTimeAdvancing)
  {
    // Pause the game briefly while we show the current hour.
    uiState = UIState.UNINTERACTIVE;

    if (animateTimeAdvancing)
    {
      timeOfDayController.AdvanceTime(
        startTime: (int) Game.time.hourOfDay,
        hours: 1,
        onHourEnd: null,
        onFinished: null);

      TimePopup.Show(hour);
      yield return new WaitForSeconds(2.5f);
    } else
      yield return TimePopup.Show(hour);

    StartCoroutine(OnHourAnimationFinished(hour));
  }

  private IEnumerator OnHourAnimationFinished(float hour) {
    // Check if any of the heroes assign themselves a task.
    List<HeroCampInfo> shuffledHeroes = heroes.Shuffle().ToList();
    foreach (HeroCampInfo hero in shuffledHeroes)
    {
      if (!hero.awake)
        continue;

      hero.portrait.SetBigStatus(Portrait.BigStatus.None);

      // If time is midnight, all heroes automatically sleep.
      if (Game.time.hourOfDay == 0)
      {
        AssignAction(hero, new CampAction_Sleep2(), selfAssigned: true);
        continue;
      } 

      // At the start of each hour, make a basic stat check.
      // On a pass, the hero awaits to be assigned a task.
      // On a fail, the hero assigns themsevles a task.
      bool awaitAssignment = hero.hero.DoSkillCheck();
      if (awaitAssignment)
        AssignAction(hero, null);
      else
      {
        // Get the hero's top 3 most desired actions.
        var top3Actions = actionManager.GetTop3(hero, heroes, Game.state);
        // Choose one of them at random.
        CampAction newAction = top3Actions.Random().Item1;
        // Assign it.
        AssignAction(hero, newAction, selfAssigned: true);
        hero.portrait.SetBigStatus(Portrait.BigStatus.Plus);
        SelectHero(hero);
        yield return SpeechBubble.Show(hero.portrait, "I feel like doing this.");
      }
    }

    if (selectedHero != null) DeselectHero(selectedHero);
    uiState = UIState.INTERACTIVE;
  }

  private void SelectHero(HeroCampInfo hero, bool showActions = true)
  {
    if (selectedHero != null)
      selectedHero.portrait.SetSelected(false);
    selectedHero = hero;
    selectedHero.portrait.SetSelected(true);
    ui.ShowStatsFor(hero.hero);
    if (showActions && hero.awake)
      ShowActionsFor(hero);
  }

  private void DeselectHero(HeroCampInfo hero)
  {
    if (hero != null && hero.portrait != null)
      hero.portrait.SetSelected(false);
    if (selectedHero == hero)
      selectedHero = null;
  }

  private void ShowActionsFor(HeroCampInfo hero)
  {
    var actionsAndAvailabilities = actionManager.GetAllAvailable(hero, heroes, Game.state);
    var options = actionsAndAvailabilities
      .Select(it => it.Item1.ToOption(hero.hero, Game.state, it.Item2)).ToList();
    var heroCopy = hero;

    actionButtons.Show(options, (option, index) => {
      var selectedAction = option.reference as CampAction;
      AssignAction(heroCopy, selectedAction);

      string message;
      if (heroCopy.selfAssignedAction != null)
      {
        if (selectedAction == heroCopy.selfAssignedAction)
        {
          message = heroCopy.action.GetAssignmentAnnouncement(hero.hero, Game.state);
          heroCopy.portrait.SetBigStatus(Portrait.BigStatus.Plus);
        }
        else
        {
          message = new string[] { "Urgh, fine.", "Do I have to?", "This sucks.", "Boo." }.Random();
          heroCopy.portrait.SetBigStatus(Portrait.BigStatus.Minus);
        }
      }
      else 
        message = heroCopy.action.GetAssignmentAnnouncement(hero.hero, Game.state);
      SpeechBubble.Show(heroCopy.portrait.transform, message, callback: null);

      actionButtons.Dismiss();
    });
  }

  private void AssignAction(HeroCampInfo hero, CampAction action, bool selfAssigned = false)
  {
    hero.action = action;
    if (selfAssigned)
      hero.selfAssignedAction = action;
    hero.portrait.SetAction(action?.titlePresentProgressive);

    if (action != null)
      SetHeroLocation(hero, action.location);

    // Check if all actions have been assigned.
    bool allHeroesReady = heroes.All(it => it.action != null);
    confirmActionsButton.SetActive(allHeroesReady);
  }

  public void ConfirmActions()
  {
    // Disable action UI.
    uiState = UIState.UNINTERACTIVE;
    confirmActionsButton.SetActive(false);
    foreach (var hero in heroes)
    {
      //      heroCampInfo.portrait.AllowCancel(false);
      DeselectHero(hero);
    }

    // Animate time advancing.
    /*  timeOfDayController.AdvanceTime(
        startTime: (int) Game.time.hourOfDay,
        hours: 1,
        onHourEnd: null,
        onFinished: OnAdvanceTimeFinished);
    }

    private void OnAdvanceTimeFinished(float timePassed, float newTime)
    {*/

    int hours = 1;

    // Update UI.
    Game.time.Advance(hours);

    // Copy current party & camp state, so that any changes made as a
    // result of one Action don't affect calculations of subsequent Actions.
    previousGameState = Game.state.DeepCopy();

    // Degrade Hero stats for every hour passed.
  /*  int hours = heroInfo
      .Select(it => it.Value.action.hours)
      .Min();*/
    Game.party.AdjustHeroStatsOverTime(hoursPassed: hours);

    foreach (var hero in heroes)
      hero.actionPending = true;

    // Sort Heroes by their Action priority, then left-to-right, then top-to-bottom.
    //heroes.Where(it => it.actionPending)
    //  .OrderBy(it => it.portrait.transform.position.y)
    //  .OrderBy(it => it.portrait.transform.position.x)
    //  .OrderBy(it => it.action.completionOrder)
    //  .ToList();

    ProcessNextAction();
  }

  /// <summary>
  /// Recursive method; keeps invoking itself until all <see cref="HeroCampInfo"/>s
  /// with pending actions are processed.
  /// </summary>
  private void ProcessNextAction()
  {
    foreach (var h in heroes)
      h.portrait.SetHighlighted(false);
//    ui.ShowStatsFor(selectedHero.hero);

    // If all actions have been completed, stop processing actions.
    if (!(heroes.Any(hero => hero.actionPending)))
    {
      FinishProcessingActions();
      return;
    }

    var hero = heroes.First(it => it.actionPending);
    hero.actionPending = false;

    hero.portrait.SetHighlighted(true);
    ui.ShowStatsFor(hero.hero);
    hero.portrait.SetAction(null);
    string message = hero.action.GetCompletionAnnouncement(hero.hero, Game.state);
    SpeechBubble.Show(hero.portrait, message, () => {
      if (hero.action is CampAction_Sleep2)
        hero.awake = false;
      StartCoroutine(hero.action.Process(hero.hero, previousGameState, Game.state, ProcessNextAction));
    });
  }

  private void FinishProcessingActions()
  {
    if (heroes.All(it => it.action is CampAction_Sleep2))
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
    foreach (var hero in heroes)
    {
      if (hero.action is not CampAction_Sleep2)
        AssignAction(hero, null);
      hero.selfAssignedAction = null;
    }

    // Update UI.
    ui.ShowPartyStats();
    uiState = UIState.INTERACTIVE;

    StartCoroutine(AnimateToNextHour(Game.time.hourOfDay, true));
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
    var heroInfo = heroes.First(it => it.portrait == portrait);
    SelectHero(heroInfo, showActions: true);
  }

  private void SetHeroLocation(
    HeroCampInfo hero,
    Location location)
  {
    var locationLayoutGroup = locationLayoutGroups[(int) location];
    hero.portrait.transform.SetParent(locationLayoutGroup.transform);

    // Force the location to update immediately, instead of waiting for next frame.
    // Fixes bug where popup text was positioned above previous position,
    // instead of new position.
    locationLayoutGroup.CalculateLayoutInputHorizontal();
    locationLayoutGroup.CalculateLayoutInputVertical();
    locationLayoutGroup.SetLayoutHorizontal();
    locationLayoutGroup.SetLayoutVertical();

    hero.location = location;
  }
}
