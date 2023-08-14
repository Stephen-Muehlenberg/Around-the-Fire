using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CampAction_Talk : CampAction
{
  public override string title => "Talk";
  public override string titlePresentProgressive => "Talking";
  public override string description => "Get to know your fellow adventurers.";
  public override CampScene.Location location => CampScene.Location.Around;

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "How are you holding up?",
      "So, tell me about yourself.",
      "What are you going to do with your share of the treasure?",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    // TODO imrpove friendship?
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
