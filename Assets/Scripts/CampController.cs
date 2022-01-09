using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main game state and logic controller.
/// </summary>
public class CampController : MonoBehaviour
{
  public enum UIState { INTERACTIVE, DRAG_IN_PROCESS, UNINTERACTIVE }

  public static List<Hero> heroes { get => campState.heroes; }
  public static Hero selectedHero;
  public static CampState campState;
  public static UIState uiState = UIState.UNINTERACTIVE;

  private static CampController singleton;

  [SerializeField] private GameObject portraitPrefab;
  [SerializeField] private HeroLocation characterPanel;
  [SerializeField] private GameObject confirmActionsButton;

  [SerializeField] private List<Sprite> TEMP_heroSprites;
  [SerializeField] private int TEMP_heroCount;

  private CampState previousState;
  private List<Hero> heroesWithPendingActions;
  
  private void Awake()
  {
    if (singleton != null) throw new System.Exception("CampController singleton already created.");
    singleton = this;

    TEMP_GenerateRandomHeroes();
  }

  private void TEMP_GenerateRandomHeroes()
  {
    int heroCount = TEMP_heroSprites.Count < TEMP_heroCount
      ? TEMP_heroSprites.Count
      : TEMP_heroCount;
    string[] names = new string[] { "Alice", "Betty", "Clair", "Diana" };
    if (heroCount > names.Length)
      heroCount = names.Length;

    campState = new CampState()
    {
      hour = 17,
      heroes = new List<Hero>(heroCount),
      firewood = Random.Range(0, 20),
      supplies = Random.Range(0, 20)
    };

    for (int i = 0; i < heroCount; i++)
    {
      campState.heroes.Add(new Hero()
      {
        name = names[i],
        icon = TEMP_heroSprites[i],
        hunger = Random.Range(15, 80),
        rest = Random.Range(15, 90),
        health = Mathf.Clamp(Random.Range(30, 170), 0, 100),
        mood = Random.Range(8, 95),
      });
    }
  }

  private void Start()
  {
    ActionManager.Initialise();
    foreach (Hero hero in heroes)
    {
      var portrait = Instantiate(portraitPrefab, characterPanel.transform)
        .GetComponent<HeroPortrait>();
      hero.portrait = portrait;
      portrait.Initialise(hero, characterPanel);
    }

    CampStatsPanel.Display(campState);
    TimeOfDayController.SetTime(campState.hour);
    FireEffects.SetState(campState.fire);

    StartCoroutine(NewHourSequence(campState.hour));
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

  private IEnumerator NewHourSequence(int hour)
  {
    yield return TimePopup.Show(hour);

    var heroesToProcess = new List<Hero>(heroes.Count);
    heroes.ForEach(it => heroesToProcess.Add(it));
    while (heroesToProcess.Count > 0)
    {
      // Choose a hero at random, so we don't always
      // have the first hero take the best available actions.
      int i = Random.Range(0, heroesToProcess.Count);
      var hero = heroesToProcess[i];
      heroesToProcess.RemoveAt(i);

      // Calculate the most desirable action for the hero.
      (HeroAction action, float weight) 
        = ActionManager.GetMostWantedAction(hero, campState);
      
      // Determine if Hero will assign themselves the task,
      // or resist the temptation.
      float randomVariance = Random.Range(0f, 1.25f);
      float outcome = (hero.mood / 100) + randomVariance - weight;
      UnityEngine.Debug.Log(hero.name + " wants to " + action.title + " (mood " + (hero.mood / 100) + " + random " + randomVariance + " - weight " + weight + " = " + outcome + ")");
      if (outcome > 0)
        continue;

      // Assign task to self.
      yield return hero.portrait.AnimateMoveTo(action.location);
      hero.SelectAction(action);
      hero.portrait.Select();
      yield return SpeechBubble.Show(hero.portrait, "I feel like doing this.");
    }

    if (selectedHero != null) selectedHero.portrait.Deselect();
    uiState = UIState.INTERACTIVE;
  }

  public static void OnActionSelected(Hero hero)
  {
    if (hero.action != null)
    {
      string message = hero.action.GetAssignmentAnnouncement(hero, campState);
      SpeechBubble.Show(hero.portrait, message, null);
    }

    bool allHeroesReady = heroes.All(it => it.action != null);
    singleton.confirmActionsButton.SetActive(allHeroesReady);
  }

  public void ConfirmActions()
  {
    // Disable action UI.
    uiState = UIState.UNINTERACTIVE;
    confirmActionsButton.SetActive(false);
    ActionList.Hide();
    heroes.ForEach(it => {
      it.portrait.AllowCancel(false);
      it.portrait.Deselect();
    });

    // Animate time advancing.
    TimeOfDayController.AdvanceTime(campState.hour, 1, null, OnAdvanceTimeFinished);
  }

  private CampState DeepCopyCurrentState()
  {
    var state = new CampState();
    heroes.ForEach(hero =>
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

  private void OnAdvanceTimeFinished(int newHour)
  {
    // Update UI.
    campState.hour = newHour;
    CampStatsPanel.Display(campState);

    // Copy current camp & party state, so that any changes made as a
    // result of one Action don't affect calculations of subsequent Actions.
    previousState = DeepCopyCurrentState();

    // Degrade Hero stats for every hour passed.
    int hours = heroes
      .Select(it => it.action.hours)
      .Min();
    DegradeStatsOverTime(hours);

    // Sort Heroes by their Action priority, then left-to-right, then top-to-bottom.
    heroesWithPendingActions = heroes
      .OrderBy(it => it.portrait.transform.position.y)
      .OrderBy(it => it.portrait.transform.position.x)
      .OrderBy(it => it.action.completionOrder)
      .ToList();

    ProcessNextAction();
  }

  private void DegradeStatsOverTime(int hours)
  {
    heroes.ForEach(it => {
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
    heroes.ForEach(it => it.portrait.Unhighlight());
    if (heroesWithPendingActions.Count == 0)
    {
      FinishActions();
      return;
    }

    var hero = heroesWithPendingActions[0];
    heroesWithPendingActions.RemoveAt(0);

    hero.portrait.Highlight();
    hero.portrait.ClearActionText();
    string message = hero.action.GetCompletionAnnouncement(hero, campState);
    SpeechBubble.Show(hero.portrait, message, () => {
      StartCoroutine(hero.action.Process(hero, previousState, campState, ProcessNextAction));
    });
  }

  private void FinishActions()
  {
    // Clear hero's actions.
    heroes.ForEach(it => it.SelectAction(null));

    // TODO show new time, and make everyone more tired.

    // Update UI.
    HeroStatsPanel.ShowStatsFor(null);
    uiState = UIState.INTERACTIVE;

    StartCoroutine(NewHourSequence(campState.hour));
  }
}
