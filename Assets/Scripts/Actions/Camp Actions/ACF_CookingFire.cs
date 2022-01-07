using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_CookingFire : HeroAction
{
  public override string title => "Build a Cooking Fire";
  public override string titlePresentProgressive => "Building a cooking fire";
  public override string description => "Make a cozy little fire for cooking.";
  public override int hours => 1;

  public override Availability AvailableFor(Hero hero, CampState campState)
  {
    if (campState.fire >= CampState.FireState.SMALL)
      return Availability.HIDDEN; // Already have a fire.
    if (campState.firewood < 4)
      return Availability.NOT_ENOUGH_WOOD;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Cooking fire's ready!",
      "Mmm... Nice and warm..."
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    currentState.firewood -= 4;
    currentState.fire = CampState.FireState.SMALL;
    FireEffects.SetState(CampState.FireState.SMALL);
    HeroStatsPanel.ShowStatsFor(hero);
    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
