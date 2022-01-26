using System;
using System.Collections;
using UnityEngine;

public class ACC_Drill : HeroAction
{
  public override string title => "Drill";
  public override string titlePresentProgressive => "Drilling";
  public override string description => "Improve the training of others.";
  public override HeroLocation location => HeroLocation.Clearing;

  public override bool AcceptedBy(Hero hero, PartyState context)
  {
    return hero.rest > 10;
  }

  public override string GetAssignmentAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Show me what you're made of!",
      "Get moving, maggots!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousContext, PartyState context, Action callback)
  {
    // TODO Show "Training++" on Heroes exercising/sparring/training.
    HeroStatsPanel.ShowStatsFor(hero);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
