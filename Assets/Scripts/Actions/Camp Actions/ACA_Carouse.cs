using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACA_Carouse : HeroAction
{
  public override string title => "Carouse";
  public override string titlePresentProgressive => "Carousing";
  public override string description => "Have a drink and some laughs with friends.";
  public override PortraitZone location => Camp.zoneAround;

  public override float GetAutoAssignWeight(Hero hero, GameState context)
    => StandardAutoAssignWeight(hero, mood: 15);

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "A toast! To the greatest heroes in the land!",
      "Did I ever tell you the one about the three legged cleric?",
      "Hahaha!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState newState, Action callback)
  {
    AdjustStats(hero, mood: 15);
    newState.party.heroes
      .Where(it => it != hero && it.location == Camp.zoneAround)
      .ToList()
      .ForEach(it => AdjustStats(it, mood: 10));
    newState.party.heroes
      .Where(it => it.location == Camp.zoneFire)
      .ToList()
      .ForEach(it => AdjustStats(it, hiddenMood: 5));

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
