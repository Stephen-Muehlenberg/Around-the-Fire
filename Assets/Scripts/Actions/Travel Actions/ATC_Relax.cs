using System;
using System.Collections;
using UnityEngine;

public class ATC_Relax : HeroAction
{
  public override string title => "Relax";
  public override string titlePresentProgressive => "Relaxing";
  public override string description => 
    "<b>+2 Mood</b> (+1 per companion)" +
    "\n<b>+1 Stamina</b>" +
    "\n\nTake it easy, enjoying the scenery and good company. More effective with more people relaxing.";
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
