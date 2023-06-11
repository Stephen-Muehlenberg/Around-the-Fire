using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class AC2A_Relax : HeroAction
{
  public override string title => "Relax";
  public override string titlePresentProgressive => "Relaxing";
  public override string description => "Take a load off and unwind.";
  public override PortraitZone location => Camp.zoneAround;

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Ahh... nice to take a load off.",
      "Maybe a quick nap...",
      "Zzz... Zzz...",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    bool hasCompanion = previousState.party.heroes
      .Where(h => h != hero)
      .Any(h => h.action is AC2A_Relax);
    if (hasCompanion)
      AdjustStats(hero, rest: 10, mood: 15);
    else
      AdjustStats(hero, rest: 15, mood: 10);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
