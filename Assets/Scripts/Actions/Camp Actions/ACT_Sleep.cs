using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACT_Sleep : HeroAction
{
  public override string title => "Sleep";
  public override string titlePresentProgressive => "Sleeping";
  public override string description => "Take a nap, or call it a night.";
  public override HeroLocation location => HeroLocation.Tent;

  public override float GetAutoAssignWeight(Hero hero, GameState context)
    => StandardAutoAssignWeight(hero, rest: 25);

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Goodnight, everyone.",
      "Sleepy time now...",
      "Zzz... Zzz...",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, rest: 25);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
