using System;
using System.Linq;
using System.Collections;
using UnityEngine;

public class ACS_Ration : HeroAction
{
  private const float RATION_SUPPLY_COST = 1;

  public override string title => "Distribute Rations";
  public override string titlePresentProgressive => "Distributing rations";
  public override string description => "Feed the party just enough to keep them going.";
  public override HeroLocation location => HeroLocation.Supplies;

  public override Availability AvailableFor(Hero hero, PartyState context)
  {
    if (context.supplies < RATION_SUPPLY_COST * context.heroes.Count)
      return Availability.NOT_ENOUGH_SUPPLIES;
    return Availability.AVAILABLE;
  }

  public override float GetAutoAssignWeight(Hero hero, PartyState context)
    => StandardAutoAssignWeight(hero, hunger: 35, rest: -5, mood: -10);

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Alright everyone, rations are ready.",
      "Get your rations. No shoving.",
      "One at a time, get your rations.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    currentState.heroes
      .ForEach(it => AdjustStats(it, hunger: 30, mood: -10));

    currentState.supplies -= RATION_SUPPLY_COST * currentState.heroes.Count;

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
