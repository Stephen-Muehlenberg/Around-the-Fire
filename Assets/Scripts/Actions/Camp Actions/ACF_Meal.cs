using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_Meal : HeroAction
{
  private static float MEAL_SUPPLY_COST = 2;

  public override string title => "Cook a Meal";
  public override string titlePresentProgressive => "Cooking a meal";
  public override string description => "Feed the whole party.";
  public override HeroLocation location => HeroLocation.Fire;
  public override int hours => 2;

  public override Availability AvailableFor(Hero hero, PartyState context)
  {
    if (context.camp.fire == CampState.FireState.NONE)
      return Availability.HIDDEN;
    return Availability.AVAILABLE;
  }

  public override float GetAutoAssignWeight(Hero hero, PartyState context)
    => StandardAutoAssignWeight(hero, hunger: 45, rest: -20);

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Come get your grub!",
      "Dinner is served.",
      "Get it while it's hot!",
      "Soup's on, everyone!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.HUNGER, 45), (Hero.Stat.REST, -20));
    currentState.heroes
      .Where(it => it != hero)
      .ToList()
      .ForEach(it => RaiseStatsAndShowPopups(it, (Hero.Stat.HUNGER, 45)));
    HeroStatsPanel.ShowStatsFor(hero);

    currentState.supplies -= MEAL_SUPPLY_COST * currentState.heroes.Count;
    CampStatsPanel.Display(currentState);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
