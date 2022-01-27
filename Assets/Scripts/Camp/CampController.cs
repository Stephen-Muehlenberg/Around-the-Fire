using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Main game state and logic controller.
/// </summary>
public class CampController : MonoBehaviour, HeroPortrait.EventsCallback
{
  public enum UIState { INTERACTIVE, DRAG_IN_PROCESS, UNINTERACTIVE }

  public static Hero selectedHero;
  public static UIState uiState = UIState.UNINTERACTIVE;

  private static CampController singleton;

  [SerializeField] private TimeOfDayController timeOfDayController;
  [SerializeField] private GameObject portraitPrefab;
  private GraphicRaycaster raycaster;
  private Canvas characterCanvas;
  [SerializeField] private HeroLocation characterPanel;
  [SerializeField] private GameObject confirmActionsButton;

  private PartyState previousState;
  private List<Hero> heroesWithPendingActions;
  
  private void Awake()
  {
    if (singleton != null) throw new System.Exception("CampController singleton already created.");
    singleton = this;
  }

  private void Start()
  {
    characterCanvas = characterPanel.GetComponentInParent<Canvas>();
    raycaster = characterPanel.GetComponentInParent<GraphicRaycaster>();

    ActionManager.Initialise();
    foreach (Hero hero in Party.heroes)
    {
      var portrait = Instantiate(portraitPrefab, characterPanel.transform)
        .GetComponent<HeroPortrait>();
      hero.portrait = portrait;
      portrait.Initialise(hero, characterPanel, this);
    }

    Party.currentState.camp = new CampState()
    {
      fire = CampState.FireState.NONE,
    };

    CampStatsPanel.Display(Party.currentState);
    timeOfDayController.SetTime(Party.time);
    FireEffects.SetState(Party.camp.fire);

    this.StartAfterDelay(0.5f, NewHourSequence(Party.time));
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }

  private IEnumerator NewHourSequence(float hour)
  {
    yield return TimePopup.Show(hour);

    var heroesToProcess = new List<Hero>(Party.heroes.Count);
    Party.heroes.ForEach(it => heroesToProcess.Add(it));
    while (heroesToProcess.Count > 0)
    {
      // Choose a hero at random, so we don't always
      // have the first hero take the best available actions.
      int i = Random.Range(0, heroesToProcess.Count);
      var hero = heroesToProcess[i];
      heroesToProcess.RemoveAt(i);

      // Calculate the most desirable action for the hero.
      (HeroAction action, float weight) 
        = ActionManager.GetMostWantedAction(hero, Party.currentState);
      
      // Determine if Hero will assign themselves the task,
      // or resist the temptation.
      float randomVariance = Random.Range(0f, 1.25f);
      float outcome = (hero.mood / 100) + randomVariance - weight;
      UnityEngine.Debug.Log(hero.name + " wants to " + action.title + " (mood " + (hero.mood / 100) + " + random " + randomVariance + " - weight " + weight + " = " + outcome + ")");
      if (outcome > 0)
        continue;

      // Assign task to self.
      yield return hero.portrait.AnimateMoveTo(action.location);
      hero.SelectAction(action, assignedBySelf: true);
      hero.portrait.Select();
      yield return SpeechBubble.Show(hero.portrait, "I feel like doing this.");
    }

    if (selectedHero != null) selectedHero.portrait.Deselect();
    uiState = UIState.INTERACTIVE;
  }

  public static void OnActionSelected(Hero hero, bool wasAssignedBySelf = false)
  {
    if (hero.action != null && !wasAssignedBySelf)
    {
      string message = hero.action.GetAssignmentAnnouncement(hero, Party.currentState);
      SpeechBubble.Show(hero.portrait, message, null);
    }

    bool allHeroesReady = Party.heroes.All(it => it.action != null);
    singleton.confirmActionsButton.SetActive(allHeroesReady);
  }

  public void ConfirmActions()
  {
    // Disable action UI.
    uiState = UIState.UNINTERACTIVE;
    confirmActionsButton.SetActive(false);
    ActionList.Hide();
    Party.heroes.ForEach(it => {
      it.portrait.AllowCancel(false);
      it.portrait.Deselect();
    });

    // Animate time advancing.
    timeOfDayController.AdvanceTime(Mathf.FloorToInt(Party.time), 1, null, OnAdvanceTimeFinished);
  }

  private PartyState DeepCopyCurrentState()
  {
    var state = new PartyState()
    {
      time = Party.time,
      heroes = new List<Hero>(Party.heroes.Count),
      supplies = Party.supplies,
      firewood = Party.firewood,
      journey = Party.journey, // TODO Might need to deep copy journey?
      camp = new CampState()
      {
        fire = Party.camp.fire,
      }
    };
    Party.heroes.ForEach(hero =>
    {
      state.heroes.Add(new Hero()
      {
        health = hero.health,
        hunger = hero.hunger,
        mood = hero.mood,
        rest = hero.rest,
        action = hero.action,
        location = hero.location
      });
    });
    return state;
  }

  private void OnAdvanceTimeFinished(float newTime)
  {
    // Update UI.
    Party.currentState.time = newTime;
    CampStatsPanel.Display(Party.currentState);

    // Copy current camp & party state, so that any changes made as a
    // result of one Action don't affect calculations of subsequent Actions.
    previousState = DeepCopyCurrentState();

    // Degrade Hero stats for every hour passed.
    int hours = Party.heroes
      .Select(it => it.action.hours)
      .Min();
    DegradeStatsOverTime(hours);

    // Sort Heroes by their Action priority, then left-to-right, then top-to-bottom.
    heroesWithPendingActions = Party.heroes
      .OrderBy(it => it.portrait.transform.position.y)
      .OrderBy(it => it.portrait.transform.position.x)
      .OrderBy(it => it.action.completionOrder)
      .ToList();

    ProcessNextAction();
  }

  private void DegradeStatsOverTime(int hours)
  {
    Party.heroes.ForEach(it => {
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
  /// Recursive method; keeps invoking itself until all <see cref="heroesWithPendingActions"/>
  /// are processed and the list emptied.
  /// </summary>
  private void ProcessNextAction()
  {
    Party.heroes.ForEach(it => it.portrait.Unhighlight());
    HeroStatsPanel.ShowStatsFor(selectedHero);
    if (heroesWithPendingActions.Count == 0)
    {
      FinishActions();
      return;
    }

    var hero = heroesWithPendingActions[0];
    heroesWithPendingActions.RemoveAt(0);

    hero.portrait.Highlight();
    HeroStatsPanel.ShowStatsFor(hero);
    hero.portrait.ClearActionText();
    string message = hero.action.GetCompletionAnnouncement(hero, Party.currentState);
    SpeechBubble.Show(hero.portrait, message, () => {
      StartCoroutine(hero.action.Process(hero, previousState, Party.currentState, ProcessNextAction));
    });
  }

  private void FinishActions()
  {
    // Clear hero's actions.
    Party.heroes.ForEach(it => it.SelectAction(null));

    // Update UI.
    HeroStatsPanel.ShowStatsFor(null);
    uiState = UIState.INTERACTIVE;

    StartCoroutine(NewHourSequence(Party.time));
  }

  public void OnPointerEnterPortrait(HeroPortrait portrait)
  {
    if (uiState != UIState.INTERACTIVE) return;
    portrait.Highlight();
    HeroStatsPanel.ShowStatsFor(portrait.hero);
  }

  public void OnPointerExitPortrait(HeroPortrait portrait)
  {
    if (uiState != UIState.INTERACTIVE) return;
    portrait.Unhighlight();
    HeroStatsPanel.ShowStatsFor(selectedHero);
  }

  public void OnPointerClickPortrait(HeroPortrait portrait)
  {
    if (uiState != UIState.INTERACTIVE) return;
    portrait.Select();
  }

  public void OnPortraitDragStart(HeroPortrait portrait, PointerEventData data)
  {
    if (uiState != UIState.INTERACTIVE) return;

    uiState = UIState.DRAG_IN_PROCESS;
    portrait.SetRaycastTarget(false);
    portrait.Select(showActions: false);

    portrait.transform.localPosition += new Vector3(
      data.delta.x,
      data.delta.y,
      0);// / transform.lossyScale.x; // Thanks to the canvas scaler we need to devide pointer delta by canvas scale to match pointer movement.

    portrait.transform.SetParent(characterCanvas.transform, true);
    portrait.transform.SetAsLastSibling();
  }

  public void OnPotraitDrag(HeroPortrait portrait, PointerEventData data)
  {
    portrait.transform.localPosition += new Vector3(
      data.delta.x,
      data.delta.y,
      0);// / transform.lossyScale.x; // Thanks to the canvas scaler we need to devide pointer delta by canvas scale to match pointer movement.
  }

  public void OnPotraitDragEnd(HeroPortrait portrait, PointerEventData data)
  {
    var results = new List<RaycastResult>();
    raycaster.Raycast(data, results);

    // Get drag destination.
    LocationZone newZone = null;
    foreach (RaycastResult result in results)
    {
      newZone = result.gameObject.GetComponentInParent<LocationZone>();
      if (newZone != null) break;
    }

    // If destination can accept this, move there.
    if (newZone != null && newZone.CanAccept(portrait))
      portrait.MoveTo(newZone);
    else
      portrait.location.CancelMove(portrait);

    portrait.SetRaycastTarget(true);
    uiState = UIState.INTERACTIVE;
  }

  public void OnPortaitCancelPressed(HeroPortrait portrait)
  {
    portrait.hero.SelectAction(null);
    if (selectedHero == portrait.hero)
      portrait.location.ShowActions(portrait.hero);
  }
}
