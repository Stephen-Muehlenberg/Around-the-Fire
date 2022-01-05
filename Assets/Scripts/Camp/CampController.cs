using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampController : MonoBehaviour
{
  public static CampController singleton;

  /// <summary>24-hour time.</summary>
  public int currentHour { get; private set; }
  public List<Hero> heroes;
  public Hero selectedHero;
  private List<ActionResult> pendingResults;

  [SerializeField] private GameObject portraitPrefab;
  [SerializeField] private HeroLocation characterPanel;
  [SerializeField] private GameObject confirmActionsButton;

  [SerializeField] private List<Sprite> TEMP_heroSprites;
  [SerializeField] private int TEMP_heroCount;

  
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
      string message = GetActionAffirmMessage(hero);
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

  private void OnAdvanceTimeFinished(int newHour)
  {
    currentHour = newHour;
    CampStatsPanel.SetStats(currentHour);

    // Calculate action results before any other changes apply.
    pendingResults = heroes
      // Calculate results.
      .Select(hero => hero.PerformAction(heroes))
      // Sort results left to right, top to bottom.
      // TODO probably have folk who have gone foraging, etc, return
      // first so it makes sense that they're affected by other actions.
      .OrderBy(result => result.hero.portrait.transform.position.y) // Top to bottom first, so it acts
      .OrderBy(result => result.hero.portrait.transform.position.x) // as a fallback for left to right.
      .ToList();

    // Time advances until one of the heroes finishes an activity.
    int hours = heroes
      .Select(it => it.action.hours)
      .Min();

    // Hero stats slowly deteriorate over time.
    heroes.ForEach(it => {
      for (int i = 0; i < hours; i++) {
        it.hunger -= Random.Range(3.5f, 4.5f);
        it.rest -= Random.Range(4.5f, 5.5f);
        it.mood -= (it.mood > 40 ? -1f : 1f) + Random.Range(-0.5f, 0.5f);
      }
    });

    ShowActionFinishMessage(pendingResults[0]);
  }

  private void ShowActionFinishMessage(ActionResult result)
  {
    // Select character; remove "action in progress" message.
    result.hero.portrait.Select();
    result.hero.portrait.ClearActionText();

    string message = result.action.GetCompletionAnnouncement(result.hero, this);    
    SpeechBubble.Show(result.hero.portrait, message, ApplyCurrentActionResults);
  }

  private void ApplyCurrentActionResults()
  {
    // Pop current action.
    var currentActionResult = pendingResults[0];
    pendingResults.RemoveAt(0);

    // Apply results, using UI popups to indicate changes.
    StartCoroutine(ApplyResult(currentActionResult));
  }

  private IEnumerator ApplyResult(ActionResult result)
  {
    UnityEngine.Debug.Log("ApplyResult(" + result.hero.name + " - " + result.action.title + ")");
    UnityEngine.Debug.Log("- deltas = " + result.partyResults.Count);
    if (result.partyResults.Any(delta => delta.health != 0))
    {
      UnityEngine.Debug.Log("- has health deltas");
      result.partyResults.ForEach(delta => {
        StatPopup.Show(delta.hero.portrait, Hero.Stat.HEALTH, delta.health);
        delta.hero.health = Mathf.Clamp(delta.hero.health + delta.health, 0, 100);
      });
      StatsPanel.ShowStatsFor(HeroPortrait.selected.hero);
      yield return new WaitForSeconds(1.5f);
    }

    if (result.partyResults.Any(delta => delta.hunger != 0))
    {
      UnityEngine.Debug.Log("- has hunger deltas");
      result.partyResults.ForEach(delta => {
        StatPopup.Show(delta.hero.portrait, Hero.Stat.HUNGER, delta.hunger);
        delta.hero.hunger = Mathf.Clamp(delta.hero.hunger + delta.hunger, 0, 100);
      });
      StatsPanel.ShowStatsFor(HeroPortrait.selected.hero);
      yield return new WaitForSeconds(1.5f);
    }

    if (result.partyResults.Any(delta => delta.mood != 0))
    {
      UnityEngine.Debug.Log("- has mood deltas");
      result.partyResults.ForEach(delta => {
        StatPopup.Show(delta.hero.portrait, Hero.Stat.MORALE, delta.mood);
        delta.hero.mood = Mathf.Clamp(delta.hero.mood + delta.mood, 0, 100);
      });
      StatsPanel.ShowStatsFor(HeroPortrait.selected.hero);
      yield return new WaitForSeconds(1.5f);
    }

    if (result.partyResults.Any(delta => delta.rest != 0))
    {
      UnityEngine.Debug.Log("- has rest deltas");
      result.partyResults.ForEach(delta => {
        StatPopup.Show(delta.hero.portrait, Hero.Stat.REST, delta.rest);
        delta.hero.rest = Mathf.Clamp(delta.hero.rest + delta.rest, 0, 100);
      });
      StatsPanel.ShowStatsFor(HeroPortrait.selected.hero);
      yield return new WaitForSeconds(1.5f);
    }

    // TODO Apply non-stat results.

    ShowNextResult();
  }

  private void ShowNextResult()
  {
    UnityEngine.Debug.Log("ShowNextResult (pending = " + pendingResults.Count + ")");
    if (pendingResults.Count > 0)
      ShowActionFinishMessage(pendingResults[0]);
    else
      FinishActions();
  }

  private void FinishActions()
  {
    UnityEngine.Debug.Log("FinishActions()");
    // Clear hero's actions.
    heroes.ForEach(it => it.portrait.SelectAction(null));

    // TODO show new time, and make everyone more tired.

    // Update UI.
    StatsPanel.ShowStatsFor(HeroPortrait.selected.hero);
  }

  /// <summary>
  /// Returns an appropriate confirmation message when the
  /// heroes selects an action.
  /// </summary>
  private static string GetActionAffirmMessage(Hero hero)
  {
    // TODO This should query the hero's list of responses, 
    // when that gets implemented.
    string[] messages = new string[] {
      "Alright.",
      "Fine.",
      "Got it.",
      "If you say so.",
      "Ok.",
      "Okey dokey.",
      "On it!",
      "Roger.",
      "Sure.",
      "Will do.",
      "With pleasure!",
      "Why not?",
    };
    return messages[Random.Range(0, messages.Length - 1)];
  }
}
