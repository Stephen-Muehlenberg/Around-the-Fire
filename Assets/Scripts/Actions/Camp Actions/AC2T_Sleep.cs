using System;
using System.Collections;
using UnityEngine;

public class AC2T_Sleep : HeroAction
{
  public override string title => "Sleep";
  public override string titlePresentProgressive => "Sleeping";
  public override string description => "Call it a night.";
  public override PortraitZone location => Camp.zoneTent;

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
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
