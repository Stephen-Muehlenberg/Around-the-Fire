using System;
using System.Collections;
using UnityEngine;

public class ACS_Snack : HeroAction
{
  private const int SNACK_SUPPLY_COST = 7;

  public override string title => "Eat a Snack";
  public override string titlePresentProgressive => "Snacking";
  public override string description => "Take the edge off your hunger.";
  public override PortraitZone location => Camp.zoneSupplies;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.party.inventory.food < SNACK_SUPPLY_COST)
      return Availability.NOT_ENOUGH_SUPPLIES;
    return Availability.AVAILABLE;
  }

  public override float GetAutoAssignWeight(Hero hero, GameState context)
    => StandardAutoAssignWeight(hero, hunger: 20, mood: 5);


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
    AdjustStats(hero, hunger: 15, mood: 5);

    currentState.party.inventory.consumeFood(SNACK_SUPPLY_COST);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
