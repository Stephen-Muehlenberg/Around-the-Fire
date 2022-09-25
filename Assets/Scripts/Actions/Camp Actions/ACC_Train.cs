using System;
using System.Collections;
using UnityEngine;

public class ACC_Train : HeroAction
{
  public override string title => "Train";
  public override string titlePresentProgressive => "Training";
  public override string description => "Improve your combat skills.";
  public override PortraitZone location => Camp.zoneClearing;

  public override bool AcceptedBy(Hero hero, GameState context)
  {
    return hero.hunger > 10
      && hero.rest > 20;
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Die, target dummy!",
      "Hyaaa! Take that!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousContext, GameState context, Action callback)
  {
    AdjustStats(hero, rest: -20, hiddenHunger: -10);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
