using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACT_Sleep : HeroAction
{
  public override string title => "Sleep";
  public override string titlePresentProgressive => "Sleeping";
  public override string description => "Take a nap, or call it a night.";

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Goodnight, everyone.",
      "Sleepy time now...",
      "Zzz... Zzz...",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.REST, 25));
    hero.portrait.Select();
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
