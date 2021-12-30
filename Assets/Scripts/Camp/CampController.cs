using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CampController : MonoBehaviour
{
  private static CampController singleton;

  /// <summary>24-hour time.</summary>
  public int hour { get; private set; }
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

    foreach (Adventurer adventurer in adventurers)
    {
      var portrait = Instantiate(portraitPrefab, characterPanel.transform)
        .GetComponent<AdventurerPortrait>();
      adventurer.portrait = portrait;
      portrait.Initialise(adventurer, characterPanel);
    }

    hour = 17;
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

  public static void OnActionSelected()
  {
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
    pendingResults = adventurers
      // Calculate results.
      .Select(adventurer => ProcessAction(adventurer))
      // Sort results left to right, top to bottom.
      // TODO probably have folk who have gone foraging, etc, return
      // first so it makes sense that they're affected by other actions.
      .OrderBy(result => result.adventurer.portrait.transform.position.y) // Top to bottom first, so it acts
      .OrderBy(result => result.adventurer.portrait.transform.position.x) // as a fallback for left to right.
      .ToList();

    ShowResult(pendingResults[0]);
  }

  private ActionResult ProcessAction(Adventurer adventurer)
  {
    // Initialise result.
    var result = new ActionResult()
    {
      adventurer = adventurer,
      action = adventurer.action,
      deltas = new List<AdventurerStatDelta>(adventurers.Count),
    };
    adventurers.ForEach(adventurer
      => result.deltas.Add(new AdventurerStatDelta() { adventurer = adventurer }));

    // Update result based on the action's properties.
    foreach (CampAction.Property property in adventurer.action.properties)
      ProcessProperty(property, result);

    return result;
  }

  private void ProcessProperty(CampAction.Property property, ActionResult result)
  {
    if (property.key.StartsWith("Team"))
    {
      if (property.key.Substring(4).Equals("Hunger"))
        AdjustTeamStat(AdventurerStat.HUNGER, property.value, result);
      else if (property.key.Substring(4).Equals("Morale"))
        AdjustTeamStat(AdventurerStat.MORALE, property.value, result);
      else if (property.key.Substring(4).Equals("Fatigue"))
        AdjustTeamStat(AdventurerStat.ENERGY, property.value, result);
      else if (property.key.Substring(4).Equals("Health"))
        AdjustTeamStat(AdventurerStat.HEALTH, property.value, result);
    }
  }

  private void AdjustTeamStat(AdventurerStat stat, int amount, ActionResult result)
  {
    foreach (AdventurerStatDelta delta in result.deltas)
      switch (stat)
      {
        case AdventurerStat.HEALTH: delta.health += amount; break;
        case AdventurerStat.HUNGER: delta.hunger += amount; break;
        case AdventurerStat.ENERGY: delta.energy += amount; break;
        case AdventurerStat.MORALE: delta.mood += amount; break;
      }
  }

  private void ShowResult(ActionResult result)
  {
    ApplyResult(result);
    pendingResults.RemoveAt(0);

    string message = "Finished.";
    var announcements = result.action.completionAnnouncements;
    if (announcements.Length > 0)
      message = announcements[Random.Range(0, announcements.Length - 1)];

    SpeechBubble.Show(result.adventurer.portrait, message, ShowNextResult);
  }

  private void ApplyResult(ActionResult result)
  {
    result.deltas.ForEach(delta => {
      Mathf.Clamp(delta.adventurer.health + delta.health, 0, 100);
      Mathf.Clamp(delta.adventurer.hunger + delta.hunger, 0, 100);
      Mathf.Clamp(delta.adventurer.rest + delta.energy, 0, 100);
      Mathf.Clamp(delta.adventurer.mood + delta.mood, 0, 100);
    });
  }

  private void ShowNextResult()
  {
    if (pendingResults.Count > 0)
      ShowResult(pendingResults[0]);
    else
      FinishActions();
  }

  private void FinishActions()
  {
    // Clear adventurer's actions.
    adventurers.ForEach(it => it.portrait.SelectAction(null));

    // Advance time.
    hour++;
    // TODO show new time, and make everyone more tired.

    // Update UI.
    StatsPanel.ShowStatsFor(AdventurerPortrait.selected.adventurer);
  }

  private enum AdventurerStat
  {
    HUNGER, ENERGY, HEALTH, MORALE
  }

  private class ActionResult
  {
    public Adventurer adventurer;
    public CampAction action;
    public List<AdventurerStatDelta> deltas;
    // TODO handle camp stats like firewood, supplies, etc.
  }

  private class AdventurerStatDelta
  {
    public Adventurer adventurer;
    public int hunger;
    public int energy;
    public int mood;
    public int health;
  }
}
