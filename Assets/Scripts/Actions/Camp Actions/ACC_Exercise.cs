using System;
using System.Collections;
using UnityEngine;

public class ACC_Exercise : HeroAction
{
  public override string title => "Exercise";
  public override string titlePresentProgressive => "Exercising";
  public override string description => "Increase your max health and stamina.";

  public override bool AcceptedBy(Hero hero, CampState context)
  {
    return hero.hunger > 10
      && hero.rest > 20;
  }

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Forty eight... Forty nine... Fifty!",
      "Huff...! Puff...!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousContext, CampState context, Action callback)
  {
    RaiseStatsAndShowPopups(hero,
      (Hero.Stat.HUNGER, -10),
      (Hero.Stat.REST, -20));
    hero.portrait.Select();
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
