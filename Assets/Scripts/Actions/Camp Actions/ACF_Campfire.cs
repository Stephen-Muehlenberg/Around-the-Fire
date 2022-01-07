using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_Campfire : HeroAction
{
  public override string title => "Build a Campfire";
  public override string titlePresentProgressive => "Building a campfire";
  public override string description => "Make a larger, more comfortable fire.";
  public override int hours => 1;

  public override Availability AvailableFor(Hero hero, CampState campState)
  {
    if (campState.fire >= CampState.FireState.MEDIUM)
      return Availability.HIDDEN; // Fire already as big or bigger.
    if (campState.firewood < (2 - campState.firewood) * 4)
      return Availability.NOT_ENOUGH_WOOD;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Camp fire's ready!",
      "Mmm... Nice and warm..."
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    if (previousState.fire == CampState.FireState.NONE)
      currentState.firewood -= 8;
    else
      currentState.fire -= 4;

    currentState.fire = CampState.FireState.MEDIUM;
    FireEffects.SetState(CampState.FireState.MEDIUM);
    hero.portrait.Select();
    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
