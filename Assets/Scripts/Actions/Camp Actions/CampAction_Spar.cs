using System;
using System.Collections;
using UnityEngine;

public class CampAction_Spar : CampAction
{
  public override string title => "Spar";
  public override string titlePresentProgressive => "Sparring";
  public override string description => "Test your mettle against a partner.";
  public override CampScene.Location location => CampScene.Location.Clearing;

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Bring it.",
      "En garde!",
      "Let's do this!",
      "Show me what you got.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, rest: -25, health: -5);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
