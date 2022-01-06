using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CampController : MonoBehaviour
{
  public static CampController singleton;

  /// <summary>24-hour time.</summary>
  public int currentHour { get; private set; }
  public List<Hero> heroes;
  public CampState campState;

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

    currentHour = 17;
  }

  private void Start()
  {
    foreach (Hero hero in heroes)
    {
      var portrait = Instantiate(portraitPrefab, characterPanel.transform)
        .GetComponent<HeroPortrait>();
      hero.portrait = portrait;
      portrait.Initialise(hero, characterPanel);
    }
    campState = new CampState() { heroes = heroes };
    FireEffects.SetState(campState.fire);

    TimeOfDayController.SetTime(currentHour);
    CampStatsPanel.SetStats(currentHour);
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

  private void TEMP_GenerateRandomHeroes()
  {
    int heroCount = TEMP_heroSprites.Count < TEMP_heroCount
      ? TEMP_heroSprites.Count
      : TEMP_heroCount;
    string[] names = new string[] { "Alice", "Betty", "Clair", "Diana" };
    if (heroCount > names.Length)
      heroCount = names.Length;

    heroes = new List<Hero>(heroCount);
    for (int i = 0; i < heroCount; i++)
    {
      heroes.Add(new Hero()
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

  public static void OnActionSelected(Hero hero)
  {
    if (hero.action != null)
    {
      string message = hero.action.GetAssignmentAnnouncement(hero, singleton);
      SpeechBubble.Show(hero.portrait, message);
    }

    singleton.confirmActionsButton.SetActive(AllHeroesReady());
  }

  private static bool AllHeroesReady()
  {
    foreach (Hero a in singleton.heroes)
      if (a.action == null)
        return false;
    return true;
  }

  public void ConfirmActions()
  {
    // Disable action UI.
    confirmActionsButton.SetActive(false);
    ActionList.Hide();
    heroes.ForEach(it => {
      it.portrait.AllowCancel(false);
      it.portrait.Deselect();
    });

    // Animate time advancing.
    TimeOfDayController.AdvanceTime(currentHour, 1, null, OnAdvanceTimeFinished);
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
    currentHour = newHour;
    CampStatsPanel.SetStats(currentHour);

    // Copy current camp & party state, so that any changes made as a
    // result of one Action don't affect calculations of subsequent Actions.
    previousState = DeepCopyCurrentState();

    // Degrade Hero stats for every hour passed.
    int hours = heroes
      .Select(it => it.action.hours)
      .Min();
    heroes.ForEach(it => {
      for (int i = 0; i < hours; i++) {
        it.hunger -= Random.Range(3.5f, 4.5f);
        it.rest -= Random.Range(4.5f, 5.5f);
        it.mood -= (it.mood > 40 ? -1f : 1f) + Random.Range(-0.5f, 0.5f);
      }
    });

    // Sort Heroes by their Action priority, then left-to-right, then top-to-bottom.
    heroesWithPendingActions = heroes
      .OrderBy(it => it.portrait.transform.position.y)
      .OrderBy(it => it.portrait.transform.position.x)
      .OrderBy(it => it.action.completionOrder)
      .ToList();

    ProcessNextAction();
  }

  /// <summary>
  /// Recursive method; keeps invoking itself until all actions processed.
  /// </summary>
  private void ProcessNextAction()
  {
    if (heroesWithPendingActions.Count == 0)
    {
      FinishActions();
      return;
    }

    var hero = heroesWithPendingActions[0];
    heroesWithPendingActions.RemoveAt(0);

    hero.portrait.Select();
    hero.portrait.ClearActionText();
    string message = hero.action.GetCompletionAnnouncement(hero, campState);
    SpeechBubble.Show(hero.portrait, message, () => {
      StartCoroutine(hero.action.Process(hero, previousState, campState, ProcessNextAction));
    });
  }

  private void FinishActions()
  {
    // Clear hero's actions.
    heroes.ForEach(it => it.portrait.SelectAction(null));

    // TODO show new time, and make everyone more tired.

    // Update UI.
    StatsPanel.ShowStatsFor(HeroPortrait.selected.hero);
  }
}
