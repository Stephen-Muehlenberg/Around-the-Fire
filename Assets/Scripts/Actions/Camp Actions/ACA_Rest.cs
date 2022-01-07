using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ACA_Rest : HeroAction
{
  public override string title => "Rest";
  public override string titlePresentProgressive => "Resting";
  public override string description => "Take a load off and unwind.";

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Ahh... nice to take a load off.",
      "Maybe a quick nap...",
      "Zzz... Zzz...",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero,
      (Hero.Stat.MORALE, 5),
      (Hero.Stat.REST, 15));
    HeroStatsPanel.ShowStatsFor(hero);
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
