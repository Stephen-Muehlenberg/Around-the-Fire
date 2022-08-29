using System;
using System.Collections;
using UnityEngine;

public class ACF_CookingFire : HeroAction
{
  public override string title => "Build a Cooking Fire";
  public override string titlePresentProgressive => "Building a cooking fire";
  public override string description => "Make a cozy little fire for cooking.";
  public override HeroLocation location => HeroLocation.Fire;
  public override int hours => 1;

  public override Availability AvailableFor(Hero hero, PartyState context)
  {
    if (context.camp.fire >= CampState.FireState.SMALL)
      return Availability.HIDDEN; // Already have a fire.
    if (context.firewood < 4)
      return Availability.NOT_ENOUGH_WOOD;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Cooking fire's ready!",
      "Mmm... Nice and warm..."
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    currentState.firewood -= 4;
    currentState.camp.fire = CampState.FireState.SMALL;
    FireEffects.SetState(CampState.FireState.SMALL);

    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
