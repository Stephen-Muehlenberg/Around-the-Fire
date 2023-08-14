using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampAction_ColdMeal2 : CampAction
{
  private const int FOOD_COST_PER_PERSON = 1;

  public override string title => "Serve Cold Meal";
  public override string titlePresentProgressive => "Serving cold meal";
  public override string description => "Prepare a basic, unheated meal for the party.";
  public override CampScene.Location location => CampScene.Location.Supplies;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.party.inventory.food < FOOD_COST_PER_PERSON * context.party.heroes.Count)
      return Availability.NOT_ENOUGH_SUPPLIES;
    return Availability.AVAILABLE;
  }

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
    currentState.party.heroes.ForEach(hero => AdjustStats(hero, hunger: 45, mood: 20));
    currentState.party.inventory.consumeFood(FOOD_COST_PER_PERSON * currentState.party.heroes.Count);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
