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

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.party.camp.fire >= CampState.FireState.MEDIUM)
      return Availability.HIDDEN; // Fire already as big or bigger.
    if (context.party.inventory.firewood < (2 - (int) context.party.camp.fire) * 4)
      return Availability.NOT_ENOUGH_WOOD;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Camp fire's ready!",
      "Mmm... Nice and warm..."
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    if (previousState.party.camp.fire == CampState.FireState.NONE)
      currentState.party.inventory.firewood -= 8;
    else
      currentState.party.camp.fire -= 4;

    currentState.party.camp.fire = CampState.FireState.MEDIUM;
    FireEffects.SetState(CampState.FireState.MEDIUM);

    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
