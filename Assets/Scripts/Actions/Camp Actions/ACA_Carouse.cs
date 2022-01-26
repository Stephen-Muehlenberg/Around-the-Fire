using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACA_Carouse : HeroAction
{
  public override string title => "Carouse";
  public override string titlePresentProgressive => "Carousing";
  public override string description => "Have a drink and some laughs with friends.";
  public override HeroLocation location => HeroLocation.Around;

  public override float GetAutoAssignWeight(Hero hero, PartyState context)
    => StandardAutoAssignWeight(hero, mood: 15);

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "A toast! To the greatest heroes in the land!",
      "Did I ever tell you the one about the three legged cleric?",
      "Hahaha!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState newState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.MORALE, 15));
    newState.heroes
      .Where(it => it != hero && it.location == HeroLocation.Around)
      .ToList()
      .ForEach(it => RaiseStatsAndShowPopups(it, (Hero.Stat.MORALE, 10)));
    newState.heroes
      .Where(it => it.location == HeroLocation.Fire)
      .ToList()
      .ForEach(it => RaiseStatsAndShowPopups(it, (Hero.Stat.MORALE, 5)));
    HeroStatsPanel.ShowStatsFor(hero);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
