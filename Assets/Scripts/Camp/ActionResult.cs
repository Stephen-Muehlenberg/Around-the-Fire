using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects of a CampAction.
/// </summary>
public class ActionResult
{
  public HeroAction action;
  public Hero hero;
  public List<HeroResults> partyResults;
  public int supplies;
  public int firewood;
  // TODO Add other camp properties as needed.

  public class HeroResults
  {
    public Hero hero;
    public int hunger;
    public int rest;
    public int mood;
    public int health;
  }

  public static ActionResult GetFor(HeroAction action, Hero hero, List<Hero> party)
  {
    // Initialise result.
    var result = new ActionResult()
    {
      hero = hero,
      action = action,
      partyResults = new List<HeroResults>(party.Count),
    };
    party.ForEach(hero
      => result.partyResults.Add(new HeroResults() { hero = hero }));

    // Update result based on the action's properties.
    // TODO Let each Action process itself and callback here.
//    foreach (CampAction.Property property in action.properties)
  //    ProcessProperty(property, hero, result);

    return result;
  }

  private static void ProcessProperty(CampAction.Property property, Hero hero, ActionResult result)
  {
    if (property.key.Length == 0) return; // TODO Make this illegal, eventually.
    string[] args = property.key.Split(' ');
    if (args.Length == 0) return; // TODO Make this illegal, eventually.

    switch (args[0].ToLower())
    {
      case "self": ApplyToSelf(args, property.value, hero, result); break;
      case "party": ApplyToAll(args, property.value, result); break;
      case "location": ApplyToLocation(args, property.value, hero.portrait.location, result); break;
      case "fire": ApplyToLocation(args, property.value, HeroLocation.Fire, result); break;
      case "around": ApplyToLocation(args, property.value, HeroLocation.Around, result); break;
      case "supplies": ApplyToLocation(args, property.value, HeroLocation.Supplies, result); break;
      case "tent": ApplyToLocation(args, property.value, HeroLocation.Tent, result); break;
      case "clearing": ApplyToLocation(args, property.value, HeroLocation.Clearing, result); break;
      case "forest": ApplyToLocation(args, property.value, HeroLocation.Forest, result); break;
    }
  }

  private static void ApplyToSelf(string[] args, int value, Hero hero, ActionResult result) {
    var heroResults = result
      .partyResults
      .First(it => it.hero == hero);
    AdjustStat(heroResults, args[1], value);
  }

  private static void ApplyToAll(string[] args, int value, ActionResult result)
  {
    result.partyResults.ForEach(it => AdjustStat(it, args[1], value));
  }

  private static void ApplyToLocation(string[] args, int value, HeroLocation location, ActionResult result)
  {
    result.partyResults
      .Where(it => it.hero.portrait.location == location)
      .ToList()
      .ForEach(it => AdjustStat(it, args[1], value));
  }

  private static void AdjustStat(HeroResults hero, string stat, int value)
  {
    switch (stat.ToLower())
    {
      case "health": hero.health += value; break;
      case "hunger": hero.hunger += value; break;
      case "morale": hero.mood += value; break;
      case "rest": hero.rest += value; break;
      default: throw new System.Exception("Unrecognized stat '" + stat + "'.");
    };
  }
}
