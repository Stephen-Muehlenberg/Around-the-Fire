using System;
using System.Collections;
using UnityEngine;

public class ACP_Forage : HeroAction
{
  public override string title => "Forage";
  public override string titlePresentProgressive => "Foraging";
  public override string description => "Search for supplies.";

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Hey. Found some grub.",
      "I'm back, y'all.",
      "Got some supplies.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.REST, -13));
    HeroStatsPanel.ShowStatsFor(hero);

    currentState.supplies += UnityEngine.Random.Range(0, 10);
    CampStatsPanel.Display(currentState);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
