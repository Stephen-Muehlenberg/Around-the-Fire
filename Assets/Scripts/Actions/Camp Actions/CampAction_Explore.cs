using System;
using System.Collections;
using UnityEngine;

public class CampAction_Explore : CampAction
{
  public override string title => "Explore";
  public override string titlePresentProgressive => "Exploring";
  public override string description => "Scout the area for threats and opportunities.";
  public override CampScene.Location location => CampScene.Location.Forest;

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, rest: -15);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
