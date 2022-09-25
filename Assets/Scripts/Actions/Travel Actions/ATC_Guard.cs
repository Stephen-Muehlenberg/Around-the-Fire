using System;
using System.Collections;
using UnityEngine;

public class ATC_Guard : HeroAction
{
  public override string title => "Guard";
  public override string titlePresentProgressive => "Guarding";
  public override string description => 
    "<b>+5 Defense</b>" +
    "\n\nKeep your weapon ready and your eyes open.";
  public override PortraitZone location => Camp.zonePerimeter; // TODO should be off to the side of the road

  public override string GetAssignmentAnnouncement(Hero hero, GameState context)
  {
    return "OK"; // TODO
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return "N/A"; // TODO Travel actions shouldn't have a completion announcement (probably?)
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    // TODO

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
