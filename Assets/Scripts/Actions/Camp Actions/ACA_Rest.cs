using System;
using System.Collections;
using UnityEngine;

public class ACA_Rest : HeroAction
{
  public override string title => "Rest";
  public override string titlePresentProgressive => "Resting";
  public override string description => "Take a load off and unwind.";
  public override HeroLocation location => HeroLocation.Around;

  public override float GetAutoAssignWeight(Hero hero, PartyState context)
    => StandardAutoAssignWeight(hero, rest: 15, mood: 5);

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Ahh... nice to take a load off.",
      "Maybe a quick nap...",
      "Zzz... Zzz...",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    AdjustStats(hero, rest: 15, hiddenMood: 5);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
