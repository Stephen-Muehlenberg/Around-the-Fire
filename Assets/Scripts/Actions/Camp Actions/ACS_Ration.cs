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
    RaiseStatsAndShowPopups(hero,
      (Hero.Stat.HUNGER, 35),
      (Hero.Stat.MORALE, -10),
      (Hero.Stat.REST, -5));
    currentState.heroes
      .Where(it => it != hero)
      .ToList()
      .ForEach(it => RaiseStatsAndShowPopups(it,
      (Hero.Stat.HUNGER, 35),
      (Hero.Stat.MORALE, -10)));
    HeroStatsPanel.ShowStatsFor(hero);

    currentState.supplies -= RATION_SUPPLY_COST * currentState.heroes.Count;
    CampStatsPanel.Display(currentState);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
