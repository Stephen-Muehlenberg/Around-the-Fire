using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CampAction_Perform : CampAction
{
  public override string title => "Perform";
  public override string titlePresentProgressive => "Performing";
  public override string description => "Entertain your friends around the fire.";
  public override CampScene.Location location => CampScene.Location.Around;

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, hiddenRest: -10);
    currentState.party.heroes
      .Where(it => it != hero && it.location == Camp.zoneAround)
      .ToList()
      .ForEach(it => AdjustStats(it, mood: 20));
    currentState.party.heroes
      .Where(it => it.location == Camp.zoneFire)
      .ToList()
      .ForEach(it => AdjustStats(it, mood: 10));
    currentState.party.heroes
      .Where(it => it.location == Camp.zoneTent || it.location == Camp.zoneSupplies)
      .ToList()
      .ForEach(it => AdjustStats(it, hiddenMood: 5));

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
