using System;
using System.Collections;
using UnityEngine;
using System.Linq;

using HeroCampInfo = CampScene.HeroCampInfo;
using System.Collections.Generic;

public class CampAction_BuildFire2 : CampAction
{
  public override string title => "Build a Campfire";
  public override string titlePresentProgressive => "Building a campfire";
  public override string description => "Make a fire for cooking and relaxing.";
  public override CampScene.Location location => CampScene.Location.Fire;

  public override Availability AvailableFor(HeroCampInfo hero, List<HeroCampInfo> party, GameState context)
  {
    if (context.camp.fire != Camp.FireState.NONE)
      return Availability.UNAVAILABLE;
    if (context.party.inventory.firewood < 4)
      return Availability.NOT_ENOUGH_WOOD;
    if (party.Any(hero => hero.action is CampAction_BuildFire2))
      return Availability.UNAVAILABLE;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Fire's ready!",
      "Mmm... Nice and warm...",
      "Ow! Burned my finger...",
      "Nothing beats a nice, warm fire."
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    currentState.party.inventory.firewood -= 4;
    currentState.camp.fire = Camp.FireState.MEDIUM;
    FireEffects.SetState(Camp.FireState.MEDIUM);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
