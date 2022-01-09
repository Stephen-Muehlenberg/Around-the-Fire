using System;
using System.Collections;
using UnityEngine;

public class ACP_Forage : HeroAction
{
  public override string title => "Forage";
  public override string titlePresentProgressive => "Foraging";
  public override string description => "Search for supplies.";
  public override HeroLocation location => HeroLocation.Perimeter;

  public override string GetAssignmentAnnouncement(Hero hero, CampState campState)
  {
    if (campState.hour > 18)
      return new string[]
      {
        "Are you sure? It's getting pretty dark out there...",
        "I dunno... It's kinda late...",
        "At this hour? Really?",
      }.Random();
    return new string[]
    {
      "I'll see what I can find.",
      "One wild boar, coming up!",
      "I think I saw some berries not far from here."
    }.Random();
  }

  public override string GetCompletionAnnouncement(Hero hero, CampState campState)
  {
    if (campState.hour > 19)
      return new string[] {
        "Getting pretty hard to find anything in the dark.",
        "I can't see anything out there.",
        "Why am I wasting my time wandering around in the dark?",
      }.Random();
    return new string[] {
      "Hey. Found some grub.",
      "I'm back, y'all.",
      "Got some supplies.",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.REST, -13));
    HeroStatsPanel.ShowStatsFor(hero);

    float foodFound = UnityEngine.Random.Range(0, 10f);
    if (currentState.hour > 19) foodFound /= 2;
    currentState.supplies += foodFound;
    CampStatsPanel.Display(currentState);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
