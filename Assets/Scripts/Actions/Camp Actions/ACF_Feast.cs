using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_Feast : HeroAction
{
  public override string title => "Prepare a Feast";
  public override string titlePresentProgressive => "Preparing a feast";
  public override string description => "Treat your party to a delicious, filling banquet!";
  public override HeroLocation location => HeroLocation.Fire;
  public override int hours => 3;

  public override Availability AvailableFor(Hero hero, GameState context)
  {
    if (context.party.camp.fire == CampState.FireState.NONE)
      return Availability.HIDDEN;
    return Availability.AVAILABLE;
  }

  public override float GetAutoAssignWeight(Hero hero, GameState context)
    => StandardAutoAssignWeight(hero, hunger: 55, rest: -35, mood: 20);

  public override string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Gather round, everyone! It's feasting time!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, hunger: 55, mood: 20, hiddenRest: -15);
    currentState.party.heroes
      .Where(it => it != hero)
      .ToList()
      .ForEach(it => AdjustStats(it, hunger: 55, mood: 20));

    // TODO Lower supplies
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
