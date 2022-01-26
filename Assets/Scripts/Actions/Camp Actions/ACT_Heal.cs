using System;
using System.Linq;
using System.Collections;
using UnityEngine;

public class ACT_Heal : HeroAction
{
  public override string title => "Heal Wounds";
  public override string titlePresentProgressive => "Healing wounds";
  public override string description => "Restore party health, prioritizing the most injured.";
  public override HeroLocation location => HeroLocation.Tent;

  public override Availability AvailableFor(Hero hero, PartyState context)
  {
    if (context.heroes.Any(it => it.health < 100))
      return Availability.AVAILABLE;
    return Availability.HIDDEN;
  }

  public override string GetAssignmentAnnouncement(Hero hero, PartyState context)
  {
    return new string[] {
      "Alright, let's take a look.",
      "Let's get you patched up.",
      "The doctor is in.",
      "Where does it hurt?",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    // TODO
    HeroStatsPanel.ShowStatsFor(hero);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
