using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACS_Ration : HeroAction
{
  public override string title => "Distribute Rations";
  public override string titlePresentProgressive => "Distributing rations";
  public override string description => "Feed the party just enough to keep them going.";

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
    currentState.heroes.ForEach(it => RaiseStatsAndShowPopups(it,
      (Hero.Stat.HUNGER, 35),
      (Hero.Stat.MORALE, -10)));
    hero.portrait.Select();
    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
