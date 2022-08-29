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
  public override HeroLocation location => HeroLocation.Perimeter; // TODO should be off to the side of the road

  public override string GetAssignmentAnnouncement(Hero hero, PartyState context)
  {
    return "OK"; // TODO
  }

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return "N/A"; // TODO Travel actions shouldn't have a completion announcement (probably?)
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    // TODO

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
