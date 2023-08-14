using System;
using System.Collections;
using UnityEngine;

public class CampAction_Train2 : CampAction
{
  public override string title => "Train";
  public override string titlePresentProgressive => "Training";
  public override string description => "Improve your combat skills.";
  public override CampScene.Location location => CampScene.Location.Clearing;

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
    AdjustStats(hero, rest: -10, hunger: -5);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
