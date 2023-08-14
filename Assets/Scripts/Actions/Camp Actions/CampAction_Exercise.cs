using System;
using System.Collections;
using UnityEngine;

public class CampAction_Exercise : CampAction
{
  public override string title => "Exercise";
  public override string titlePresentProgressive => "Exercising";
  public override string description => "Increase your max health and stamina.";
  public override CampScene.Location location => CampScene.Location.Clearing;

  public override bool AcceptedBy(Hero hero, GameState context)
  {
    return hero.hunger > 10
      && hero.rest > 20;
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Forty eight... Forty nine... Fifty!",
      "Huff...! Puff...!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousContext, GameState context, Action callback)
  {
    AdjustStats(hero, rest: -20, hiddenHunger: -10);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
