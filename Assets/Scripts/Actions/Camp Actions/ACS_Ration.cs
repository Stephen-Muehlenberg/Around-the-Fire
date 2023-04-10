using System;
using System.Linq;
using System.Collections;
using UnityEngine;

public class ACS_Ration : HeroAction
{
  private const int RATION_SUPPLY_COST = 1;

  public override string title => "Distribute Rations";
  public override string titlePresentProgressive => "Distributing rations";
  public override string description => "Feed the party just enough to keep them going.";
  public override PortraitZone location => Camp.zoneSupplies;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.party.inventory.food < RATION_SUPPLY_COST * context.party.heroes.Count)
      return Availability.NOT_ENOUGH_SUPPLIES;
    return Availability.AVAILABLE;
  }

  public override float GetAutoAssignWeight(Hero hero, GameState context)
    => StandardAutoAssignWeight(hero, hunger: 35, rest: -5, mood: -10);

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Alright everyone, rations are ready.",
      "Get your rations. No shoving.",
      "One at a time, get your rations.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    currentState.party.heroes
      .ForEach(it => AdjustStats(it, hunger: 30, mood: -10));

    currentState.party.inventory.consumeFood(RATION_SUPPLY_COST * currentState.party.heroes.Count);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
