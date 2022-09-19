using System;
using System.Collections;
using UnityEngine;

public class ACF_Bonfire : HeroAction
{
  public override string title => "Build a Bonfire";
  public override string titlePresentProgressive => "Building a bonfire";
  public override string description => "A roaring fire to raise your spirits.";
  public override HeroLocation location => HeroLocation.Fire;
  public override int hours => 1;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.party.camp.fire == CampState.FireState.LARGE)
      return Availability.HIDDEN;
    if (context.party.inventory.firewood < (3 - (int) context.party.camp.fire) * 4)
      return Availability.NOT_ENOUGH_WOOD;
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
    currentState.party.inventory.firewood += ((int) previousState.party.camp.fire -3) * 4;
    currentState.party.camp.fire = CampState.FireState.LARGE;
    FireEffects.SetState(CampState.FireState.LARGE);

    // TODO Lower supplies

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
