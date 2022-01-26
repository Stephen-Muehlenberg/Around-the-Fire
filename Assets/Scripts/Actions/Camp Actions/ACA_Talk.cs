using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACA_Talk : HeroAction
{
  public override string title => "Talk";
  public override string titlePresentProgressive => "Talking";
  public override string description => "Get to know your fellow adventurers.";
  public override HeroLocation location => HeroLocation.Around;

  public override string GetCompletionAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "How are you holding up?",
      "So, tell me about yourself.",
      "What are you going to do with your share of the treasure?",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    // TODO imrpove friendship?
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
