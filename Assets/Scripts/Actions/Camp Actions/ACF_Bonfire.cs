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

  public override Availability AvailableFor(Hero hero, PartyState context)
  {
    if (context.camp.fire == CampState.FireState.LARGE)
      return Availability.HIDDEN;
    if (context.firewood < (3 - (int) context.camp.fire) * 4)
      return Availability.NOT_ENOUGH_WOOD;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Bonfire's up!",
      "So warm. So pretty.",
      "Yes... Burn, burn!"
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    currentState.firewood += ((int) previousState.camp.fire -3) * 4;
    currentState.camp.fire = CampState.FireState.LARGE;
    FireEffects.SetState(CampState.FireState.LARGE);

    // TODO Lower supplies

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
