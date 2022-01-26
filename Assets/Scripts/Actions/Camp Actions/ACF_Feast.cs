using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_Feast : HeroAction
{
  public override string title => "Prepare a Feast";
  public override string titlePresentProgressive => "Preparing a feast";
  public override string description => "Treat your party to a delicious, filling banquet!";
  public override HeroLocation location => HeroLocation.Fire;
  public override int hours => 3;

  public override Availability AvailableFor(Hero hero, PartyState context)
  {
    if (context.camp.fire == CampState.FireState.NONE)
      return Availability.HIDDEN;
    return Availability.AVAILABLE;
  }

  public override float GetAutoAssignWeight(Hero hero, PartyState context)
    => StandardAutoAssignWeight(hero, hunger: 55, rest: -35, mood: 20);

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Gather round, everyone! It's feasting time!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, 
      (Hero.Stat.HUNGER, 55),
      (Hero.Stat.MORALE, 20),
      (Hero.Stat.REST, -35));
    currentState.heroes
      .Where(it => it != hero)
      .ToList()
      .ForEach(it => RaiseStatsAndShowPopups(it,
      (Hero.Stat.HUNGER, 55),
      (Hero.Stat.MORALE, 20)));
    HeroStatsPanel.ShowStatsFor(hero);
    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
