using System;
using System.Collections;
using UnityEngine;

public class ACP_Wood : HeroAction
{
  public override string title => "Gather Wood";
  public override string titlePresentProgressive => "Gathering wood";
  public override string description => "Find fuel for the fire.";
  public override PortraitZone location => Camp.zonePerimeter;

  public override string GetAssignmentAnnouncement(Hero hero, GameState context)
  {
    if (context.world.time.hourOfDay >= 19)
      return new string[]
      {
        "Are you sure? It's getting pretty dark out there...",
        "I dunno... It's kinda late...",
        "At this hour? Really?",
      }.Random();
    return base.GetAssignmentAnnouncement(hero, context);
  }

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    if (context.world.time.hourOfDay >= 20)
      return new string[] {
        "Getting pretty hard to find anything in the dark.",
        "I can't see anything out there.",
        "Why am I wasting my time wandering around in the dark?",
      }.Random();
    return new string[] {
      "Alright, let's get this fire started.",
      "I'm back.",
      "More wood for the pile.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, rest: -12);

    int woodFound = UnityEngine.Random.Range(4, 12);
    if (currentState.world.time.hourOfDay >= 20) woodFound -= 4;
    currentState.party.inventory.firewood += woodFound;
    
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
