using System;
using System.Collections;
using UnityEngine;

public class ATC_Forage : HeroAction
{
  public override string title => "Forage";
  public override string titlePresentProgressive => "Foraging";
  public override string description => 
    "<b>+1-3 Supplies (30%)</b>" +
    "\n\nSearch for food while on the move.";
  public override HeroLocation location => HeroLocation.Perimeter; // TODO should be off to the side of the road

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
