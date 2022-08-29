using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACA_Perform : HeroAction
{
  public override string title => "Perform";
  public override string titlePresentProgressive => "Performing";
  public override string description => "Entertain your friends around the fire.";
  public override HeroLocation location => HeroLocation.Around;

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    AdjustStats(hero, hiddenRest: -10);
    currentState.heroes
      .Where(it => it != hero && it.location == HeroLocation.Around)
      .ToList()
      .ForEach(it => AdjustStats(it, mood: 20));
    currentState.heroes
      .Where(it => it.location == HeroLocation.Fire)
      .ToList()
      .ForEach(it => AdjustStats(it, mood: 10));
    currentState.heroes
      .Where(it => it.location == HeroLocation.Tent || it.location == HeroLocation.Supplies)
      .ToList()
      .ForEach(it => AdjustStats(it, hiddenMood: 5));

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
