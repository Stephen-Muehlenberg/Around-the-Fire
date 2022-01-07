using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_Feast : HeroAction
{
  public override string title => "Prepare a Feast";
  public override string titlePresentProgressive => "Preparing a feast";
  public override string description => "Treat your party to a delicious, filling banquet!";
  public override int hours => 3;

  public override Availability AvailableFor(Hero hero, CampState campState)
  {
    if (campState.fire == CampState.FireState.NONE)
      return Availability.HIDDEN;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Gather round, everyone! It's feasting time!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
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
    hero.portrait.Select();
    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
