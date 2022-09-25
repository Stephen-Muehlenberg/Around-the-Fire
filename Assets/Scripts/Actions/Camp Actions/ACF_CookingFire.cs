using System;
using System.Collections;
using UnityEngine;

public class ACF_CookingFire : HeroAction
{
  public override string title => "Build a Cooking Fire";
  public override string titlePresentProgressive => "Building a cooking fire";
  public override string description => "Make a cozy little fire for cooking.";
  public override PortraitZone location => Camp.zoneFire;
  public override int hours => 1;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.camp.fire >= Camp.FireState.SMALL)
      return Availability.HIDDEN; // Already have a fire.
    if (context.party.inventory.firewood < 4)
      return Availability.NOT_ENOUGH_WOOD;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Cooking fire's ready!",
      "Mmm... Nice and warm..."
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    currentState.party.inventory.firewood -= 4;
    currentState.camp.fire = Camp.FireState.SMALL;
    FireEffects.SetState(Camp.FireState.SMALL);

    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
