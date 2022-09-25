using System;
using System.Collections;
using UnityEngine;

public class ACC_Drill : HeroAction
{
  public override string title => "Drill";
  public override string titlePresentProgressive => "Drilling";
  public override string description => "Improve the training of others.";
  public override PortraitZone location => Camp.zoneClearing;

  public override bool AcceptedBy(Hero hero, GameState context)
  {
    return hero.rest > 10;
  }

  public override string GetAssignmentAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Show me what you're made of!",
      "Get moving, maggots!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousContext, GameState context, Action callback)
  {
    // TODO Show "Training++" on Heroes exercising/sparring/training.

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
