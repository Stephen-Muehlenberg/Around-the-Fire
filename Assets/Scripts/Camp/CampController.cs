using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampController : MonoBehaviour
{
  private static CampController singleton;

  /// <summary>24-hour time.</summary>
  public int currentHour { get; private set; }
  public List<Adventurer> adventurers;
  public Adventurer selectedAdventurer;
  private List<ActionResult> pendingResults;

  [SerializeField] private GameObject portraitPrefab;
  [SerializeField] private CampLocation characterPanel;
  [SerializeField] private GameObject confirmActionsButton;

  [SerializeField] private List<Sprite> TEMP_adventurerSprites;
  [SerializeField] private int TEMP_adventurerCount;

  private void Awake()
  {
    if (singleton != null) throw new System.Exception("CampController singleton already created.");
    singleton = this;

    TEMP_GenerateRandomAdventurers();

    currentHour = 17;
  }

  private void Start()
  {
    foreach (Adventurer adventurer in adventurers)
    {
      var portrait = Instantiate(portraitPrefab, characterPanel.transform)
        .GetComponent<AdventurerPortrait>();
      adventurer.portrait = portrait;
      portrait.Initialise(adventurer, characterPanel);
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

  private void TEMP_GenerateRandomAdventurers()
  {
    int adventurerCount = TEMP_adventurerSprites.Count < TEMP_adventurerCount
      ? TEMP_adventurerSprites.Count
      : TEMP_adventurerCount;
    string[] names = new string[] { "Alice", "Betty", "Clair", "Diana" };
    if (adventurerCount > names.Length)
      adventurerCount = names.Length;

    adventurers = new List<Adventurer>(adventurerCount);
    for (int i = 0; i < adventurerCount; i++)
    {
      adventurers.Add(new Adventurer()
      {
        name = names[i],
        icon = TEMP_adventurerSprites[i],
        hunger = Random.Range(15, 80),
        rest = Random.Range(15, 90),
        health = Mathf.Clamp(Random.Range(30, 170), 0, 100),
        mood = Random.Range(8, 95),
      });
    }
  }

  public static void OnActionSelected(Adventurer adventurer)
  {
    if (adventurer.action != null)
    {
      string message = GetActionAffirmMessage(adventurer);
      SpeechBubble.Show(adventurer.portrait, message);
    }

    singleton.confirmActionsButton.SetActive(AllAdventurersReady());
  }

  private static bool AllAdventurersReady()
  {
    foreach (Adventurer a in singleton.adventurers)
      if (a.action == null)
        return false;
    return true;
  }

  public void ConfirmActions()
  {
    // Disable action UI.
    confirmActionsButton.SetActive(false);
    ActionList.Hide();
    adventurers.ForEach(it => {
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
    pendingResults = adventurers
      // Calculate results.
      .Select(adventurer => adventurer.PerformAction(adventurers))
      // Sort results left to right, top to bottom.
      // TODO probably have folk who have gone foraging, etc, return
      // first so it makes sense that they're affected by other actions.
      .OrderBy(result => result.adventurer.portrait.transform.position.y) // Top to bottom first, so it acts
      .OrderBy(result => result.adventurer.portrait.transform.position.x) // as a fallback for left to right.
      .ToList();

    // Time advances until one of the adventurers finishes an activity.
    int hours = adventurers
      .Select(it => it.action.hours)
      .Min();

    // Adventurer stats slowly deteriorate over time.
    adventurers.ForEach(it => {
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
    result.adventurer.portrait.Select();
    result.adventurer.portrait.ClearActionText();

    // TODO Either have some set of default fallback messages,
    // or ensure there's always an action-specific message.
    string message = "Finished.";
    var announcements = result.action.completionAnnouncements;
    if (announcements.Length > 0)
      message = announcements[Random.Range(0, announcements.Length)];

    SpeechBubble.Show(result.adventurer.portrait, message, ApplyCurrentActionResults);
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
    UnityEngine.Debug.Log("ApplyResult(" + result.adventurer.name + " - " + result.action.title + ")");
    UnityEngine.Debug.Log("- deltas = " + result.partyResults.Count);
    if (result.partyResults.Any(delta => delta.health != 0))
    {
      UnityEngine.Debug.Log("- has health deltas");
      result.partyResults.ForEach(delta => {
        StatPopup.Show(delta.adventurer.portrait, Adventurer.Stat.HEALTH, delta.health);
        delta.adventurer.health = Mathf.Clamp(delta.adventurer.health + delta.health, 0, 100);
      });
      StatsPanel.ShowStatsFor(AdventurerPortrait.selected.adventurer);
      yield return new WaitForSeconds(1.5f);
    }

    if (result.partyResults.Any(delta => delta.hunger != 0))
    {
      UnityEngine.Debug.Log("- has hunger deltas");
      result.partyResults.ForEach(delta => {
        StatPopup.Show(delta.adventurer.portrait, Adventurer.Stat.HUNGER, delta.hunger);
        delta.adventurer.hunger = Mathf.Clamp(delta.adventurer.hunger + delta.hunger, 0, 100);
      });
      StatsPanel.ShowStatsFor(AdventurerPortrait.selected.adventurer);
      yield return new WaitForSeconds(1.5f);
    }

    if (result.partyResults.Any(delta => delta.mood != 0))
    {
      UnityEngine.Debug.Log("- has mood deltas");
      result.partyResults.ForEach(delta => {
        StatPopup.Show(delta.adventurer.portrait, Adventurer.Stat.MORALE, delta.mood);
        delta.adventurer.mood = Mathf.Clamp(delta.adventurer.mood + delta.mood, 0, 100);
      });
      StatsPanel.ShowStatsFor(AdventurerPortrait.selected.adventurer);
      yield return new WaitForSeconds(1.5f);
    }

    if (result.partyResults.Any(delta => delta.rest != 0))
    {
      UnityEngine.Debug.Log("- has rest deltas");
      result.partyResults.ForEach(delta => {
        StatPopup.Show(delta.adventurer.portrait, Adventurer.Stat.REST, delta.rest);
        delta.adventurer.rest = Mathf.Clamp(delta.adventurer.rest + delta.rest, 0, 100);
      });
      StatsPanel.ShowStatsFor(AdventurerPortrait.selected.adventurer);
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
    // Clear adventurer's actions.
    adventurers.ForEach(it => it.portrait.SelectAction(null));

    // TODO show new time, and make everyone more tired.

    // Update UI.
    StatsPanel.ShowStatsFor(AdventurerPortrait.selected.adventurer);
  }

  /// <summary>
  /// Returns an appropriate confirmation message when the
  /// adventurer selects an action.
  /// </summary>
  private static string GetActionAffirmMessage(Adventurer adventurer)
  {
    // TODO This should query the adventurer's list of responses, 
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
