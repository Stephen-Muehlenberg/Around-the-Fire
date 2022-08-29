using System;
using System.Collections;
using UnityEngine;

public class ACF_Campfire : HeroAction
{
  public override string title => "Build a Campfire";
  public override string titlePresentProgressive => "Building a campfire";
  public override string description => "Make a larger, more comfortable fire.";
  public override HeroLocation location => HeroLocation.Fire;
  public override int hours => 1;

  public override Availability AvailableFor(Hero hero, PartyState context)
  {
    if (context.camp.fire >= CampState.FireState.MEDIUM)
      return Availability.HIDDEN; // Fire already as big or bigger.
    if (context.firewood < (2 - (int) context.camp.fire) * 4)
      return Availability.NOT_ENOUGH_WOOD;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Camp fire's ready!",
      "Mmm... Nice and warm..."
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    if (previousState.camp.fire == CampState.FireState.NONE)
      currentState.firewood -= 8;
    else
      currentState.camp.fire -= 4;

    currentState.camp.fire = CampState.FireState.MEDIUM;
    FireEffects.SetState(CampState.FireState.MEDIUM);

    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
