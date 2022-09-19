using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_Meal : HeroAction
{
  private const float MEAL_SUPPLY_COST = 2;

  public override string title => "Cook a Meal";
  public override string titlePresentProgressive => "Cooking a meal";
  public override string description => "Feed the whole party.";
  public override HeroLocation location => HeroLocation.Fire;
  public override int hours => 2;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.party.camp.fire == CampState.FireState.NONE)
      return Availability.HIDDEN;
    return Availability.AVAILABLE;
  }

  public override float GetAutoAssignWeight(Hero hero, GameState context)
    => StandardAutoAssignWeight(hero, hunger: 45, rest: -20);

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
    AdjustStats(hero, hunger: 45, hiddenRest: -10);
    currentState.party.heroes
      .Where(it => it != hero)
      .ToList()
      .ForEach(it => AdjustStats(it, hunger: 45));

    currentState.party.inventory.supplies -= MEAL_SUPPLY_COST * currentState.party.heroes.Count;

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
