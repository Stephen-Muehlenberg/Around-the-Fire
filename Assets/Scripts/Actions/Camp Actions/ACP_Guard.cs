using System;
using System.Collections;
using UnityEngine;

public class ACP_Guard : HeroAction
{
  public override string title => "Stand Watch";
  public override string titlePresentProgressive => "Standing watch";
  public override string description => "Guard the camp.";
  public override PortraitZone location => Camp.zonePerimeter;

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, rest: -10);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
