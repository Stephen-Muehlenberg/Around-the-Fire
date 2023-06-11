using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public class AC2F_BuildFire : HeroAction
{
  public override string title => "Build a Campfire";
  public override string titlePresentProgressive => "Building a campfire";
  public override string description => "Make a fire for cooking and relaxing.";
  public override PortraitZone location => Camp.zoneFire;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.camp.fire != Camp.FireState.NONE)
      return Availability.UNAVAILABLE;
    if (context.party.inventory.firewood < 4)
      return Availability.NOT_ENOUGH_WOOD;
    if (context.party.heroes.Any(hero => hero.action is AC2F_BuildFire))
      return Availability.UNAVAILABLE;
    return Availability.AVAILABLE;
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Fire's ready!",
      "Mmm... Nice and warm...",
      "Ow! Burned my finger...",
      "Nothing beats a nice, warm fire."
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    currentState.party.inventory.firewood -= 4;
    currentState.camp.fire = Camp.FireState.MEDIUM;
    FireEffects.SetState(Camp.FireState.MEDIUM);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
