using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_Bonfire : HeroAction
{
  public override string title => "Build a Bonfire";
  public override string titlePresentProgressive => "Building a bonfire";
  public override string description => "A roaring fire to raise your spirits.";
  public override int hours => 1;

  public override Availability AvailableFor(Hero hero, CampState campState)
  {
    if (campState.fire == CampState.FireState.LARGE)
      return Availability.HIDDEN;
    if (campState.firewood < (3 - (int) campState.fire) * 4)
      return Availability.NOT_ENOUGH_WOOD;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Bonfire's up!",
      "So warm. So pretty.",
      "Yes... Burn, burn!"
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    currentState.firewood += ((int) previousState.fire -3) * 4;
    currentState.fire = CampState.FireState.LARGE;
    FireEffects.SetState(CampState.FireState.LARGE);
    hero.portrait.Select();
    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
