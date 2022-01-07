using System;
using System.Collections;
using UnityEngine;

public class ACP_Wood : HeroAction
{
  public override string title => "Gather Wood";
  public override string titlePresentProgressive => "Gathering wood";
  public override string description => "Find fuel for the fire.";

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Alright, let's get this fire started.",
      "I'm back.",
      "More wood for the pile.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.REST, -15));
    HeroStatsPanel.ShowStatsFor(hero);

    currentState.firewood += UnityEngine.Random.Range(4, 12);
    CampStatsPanel.Display(currentState);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
