using System;
using System.Collections;
using UnityEngine;

public class ACC_Train : HeroAction
{
  public override string title => "Train";
  public override string titlePresentProgressive => "Training";
  public override string description => "Improve your combat skills.";
  public override HeroLocation location => HeroLocation.Clearing;

  public override bool AcceptedBy(Hero hero, CampState campState)
  {
    return hero.hunger > 10
      && hero.rest > 20;
  }

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Die, target dummy!",
      "Hyaaa! Take that!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousContext, CampState context, Action callback)
  {
    RaiseStatsAndShowPopups(hero,
      (Hero.Stat.HUNGER, -10),
      (Hero.Stat.REST, -20));
    HeroStatsPanel.ShowStatsFor(hero);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
