using System;
using System.Collections;
using UnityEngine;

public class ACP_Explore : HeroAction
{
  public override string title => "Explore";
  public override string titlePresentProgressive => "Exploring";
  public override string description => "Scout the area for threats and opportunities.";
  public override PortraitZone location => Camp.zonePerimeter;

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    AdjustStats(hero, rest: -15);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
