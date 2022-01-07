using System;
using System.Collections;
using UnityEngine;

public class ACC_Spar : HeroAction
{
  public override string title => "Spar";
  public override string titlePresentProgressive => "Sparring";
  public override string description => "Test your mettle against a partner.";

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Bring it.",
      "En garde!",
      "Let's do this!",
      "Show me what you got.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero,
      (Hero.Stat.HEALTH, -5),
      (Hero.Stat.REST, -25));
    HeroStatsPanel.ShowStatsFor(hero);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
