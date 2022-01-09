using System;
using System.Collections;
using UnityEngine;

public class ACS_Snack : HeroAction
{
  private static float SNACK_SUPPLY_COST = 7;

  public override string title => "Eat a Snack";
  public override string titlePresentProgressive => "Snacking";
  public override string description => "Take the edge off your hunger.";
  public override HeroLocation location => HeroLocation.Supplies;

  public override Availability AvailableFor(Hero hero, CampState context)
  {
    if (context.supplies < SNACK_SUPPLY_COST)
      return Availability.NOT_ENOUGH_SUPPLIES;
    return Availability.AVAILABLE;
  }

  public override float GetAutoAssignWeight(Hero hero, CampState campState)
    => StandardAutoAssignWeight(hero, hunger: 20, mood: 5);


  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Alright everyone, rations are ready.",
      "Get your rations. No shoving.",
      "One at a time, get your rations.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero,
      (Hero.Stat.HUNGER, 20),
      (Hero.Stat.MORALE, 5));
    HeroStatsPanel.ShowStatsFor(hero);

    currentState.supplies -= SNACK_SUPPLY_COST;
    CampStatsPanel.Display(currentState);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
