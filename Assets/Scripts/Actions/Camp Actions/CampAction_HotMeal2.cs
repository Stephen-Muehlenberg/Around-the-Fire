using System;
using System.Collections;
using UnityEngine;

public class CampAction_HotMeal2 : CampAction
{
  private const int FOOD_COST_PER_PERSON = 1;

  public override string title => "Cook Hot Meal";
  public override string titlePresentProgressive => "Cooking a meal";
  public override string description => "Prepare a nice, warm meal for the party.";
  public override CampScene.Location location => CampScene.Location.Fire;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.camp.fire == Camp.FireState.NONE)
      return Availability.NEEDS_A_FIRE;
    if (context.party.inventory.food < FOOD_COST_PER_PERSON * context.party.heroes.Count)
      return Availability.NOT_ENOUGH_SUPPLIES;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Come get your grub!",
      "Dinner is served.",
      "Get it while it's hot!",
      "Soup's on, everyone!",
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
