using System;
using System.Linq;
using System.Collections;
using UnityEngine;

public class ACT_ReceiveHealing : HeroAction
{
  public override string title => "Receive Healing";
  public override string titlePresentProgressive => "Receiving healing";
  public override string description => "Become a priority target for Heal Wounds, and receive better treatment.";
  public override PortraitZone location => Camp.zoneTent;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (hero.health < 100)
      return Availability.AVAILABLE;
    return Availability.HIDDEN;
  }

  public override string GetAssignmentAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "I don't feel so good...",
      "Ow, ow, ow! Medic!",
      "Will it hurt?",
    }.Random();
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    if (context.party.heroes.Any(it => it.action is ACT_Heal))
      return new string[] {
        "Ahhh... That's better.",
        "Ow! Careful with that needle, doc!",
        "Feeling much better, thanks.",
      }.Random();
    else
      return new string[] {
        "Medic? Medic...?",
        "Where's the damn doctor?",
        "Oooh, please, somebody, it hurts...",
      }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
