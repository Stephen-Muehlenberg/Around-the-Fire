using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

using HeroCampInfo = CampScene.HeroCampInfo;

public class CampAction_Bonfire : CampAction
{
  public override string title => "Build a Bonfire";
  public override string titlePresentProgressive => "Building a bonfire";
  public override string description => "A roaring fire to raise your spirits.";
  public override CampScene.Location location => CampScene.Location.Fire;
  public override int hours => 1;

  public override Availability AvailableFor(HeroCampInfo hero, List<HeroCampInfo> party, GameState context)
  {
    if (context.camp.fire == Camp.FireState.LARGE)
      return Availability.HIDDEN;
    if (context.party.inventory.firewood < (3 - (int) context.camp.fire) * 4)
      return Availability.NOT_ENOUGH_WOOD;
    if (party.Any(it => it.action is CampAction_Bonfire))
      return Availability.UNAVAILABLE;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Bonfire's up!",
      "So warm. So pretty.",
      "Yes... Burn, burn!"
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    currentState.party.inventory.firewood += ((int) previousState.camp.fire -3) * 4;
    currentState.camp.fire = Camp.FireState.LARGE;
    FireEffects.SetState(Camp.FireState.LARGE);

    // TODO Lower supplies

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
