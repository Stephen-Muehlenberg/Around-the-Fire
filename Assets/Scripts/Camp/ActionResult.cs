using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects of a CampAction.
/// </summary>
public class ActionResult
{
  public CampAction action;
  public Adventurer adventurer;
  public List<AdventurerResult> partyResults;
  public int supplies;
  public int firewood;
  // TODO Add other camp properties as needed.

  public class AdventurerResult
  {
    public Adventurer adventurer;
    public int hunger;
    public int rest;
    public int mood;
    public int health;
  }

  public static ActionResult GetFor(CampAction action, Adventurer adventurer, List<Adventurer> party)
  {
    // Initialise result.
    var result = new ActionResult()
    {
      adventurer = adventurer,
      action = action,
      partyResults = new List<AdventurerResult>(party.Count),
    };
    party.ForEach(adventurer
      => result.partyResults.Add(new AdventurerResult() { adventurer = adventurer }));

    // Update result based on the action's properties.
    foreach (CampAction.Property property in action.properties)
      ProcessProperty(property, adventurer, result);

    return result;
  }

  private static void ProcessProperty(CampAction.Property property, Adventurer adventurer, ActionResult result)
  {
    if (property.key.Length == 0) return; // TODO Make this illegal, eventually.
    string[] args = property.key.Split(' ');
    if (args.Length == 0) return; // TODO Make this illegal, eventually.

    switch (args[0].ToLower())
    {
      case "self": ApplyToSelf(args, property.value, adventurer, result); break;
      case "party": ApplyToAll(args, property.value, result); break;
      case "location": ApplyToLocation(args, property.value, adventurer.portrait.location, result); break;
      case "fire": ApplyToLocation(args, property.value, CampLocation.Fire, result); break;
      case "around": ApplyToLocation(args, property.value, CampLocation.Around, result); break;
      case "supplies": ApplyToLocation(args, property.value, CampLocation.Supplies, result); break;
      case "tent": ApplyToLocation(args, property.value, CampLocation.Tent, result); break;
      case "clearing": ApplyToLocation(args, property.value, CampLocation.Clearing, result); break;
      case "forest": ApplyToLocation(args, property.value, CampLocation.Forest, result); break;
    }
  }

  private static void ApplyToSelf(string[] args, int value, Adventurer adventurer, ActionResult result) {
    var adventurerResult = result
      .partyResults
      .First(it => it.adventurer == adventurer);
    AdjustStat(adventurerResult, args[1], value);
  }

  private static void ApplyToAll(string[] args, int value, ActionResult result)
  {
    result.partyResults.ForEach(it => AdjustStat(it, args[1], value));
  }

  private static void ApplyToLocation(string[] args, int value, CampLocation location, ActionResult result)
  {
    result.partyResults
      .Where(it => it.adventurer.portrait.location == location)
      .ToList()
      .ForEach(it => AdjustStat(it, args[1], value));
  }

  private static void AdjustStat(AdventurerResult adventurer, string stat, int value)
  {
    switch (stat.ToLower())
    {
      case "health": adventurer.health += value; break;
      case "hunger": adventurer.hunger += value; break;
      case "morale": adventurer.mood += value; break;
      case "rest": adventurer.rest += value; break;
      default: throw new System.Exception("Unrecognized stat '" + stat + "'.");
    };
  }
}
