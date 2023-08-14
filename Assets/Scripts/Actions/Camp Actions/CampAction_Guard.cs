using System;
using System.Collections;
using UnityEngine;

public class CampAction_Guard : CampAction
{
  public override string title => "Stand Watch";
  public override string titlePresentProgressive => "Standing watch";
  public override string description => "Guard the camp.";
  public override CampScene.Location location => CampScene.Location.Forest;

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, rest: -10);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
