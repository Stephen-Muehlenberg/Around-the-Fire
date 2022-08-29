using System;
using System.Collections;
using UnityEngine;

public class ACC_Exercise : HeroAction
{
  public override string title => "Exercise";
  public override string titlePresentProgressive => "Exercising";
  public override string description => "Increase your max health and stamina.";
  public override HeroLocation location => HeroLocation.Clearing;

  public override bool AcceptedBy(Hero hero, PartyState context)
  {
    return hero.hunger > 10
      && hero.rest > 20;
  }

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Forty eight... Forty nine... Fifty!",
      "Huff...! Puff...!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousContext, PartyState context, Action callback)
  {
    AdjustStats(hero, rest: -20, hiddenHunger: -10);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
