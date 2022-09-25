using System;
using System.Collections;
using UnityEngine;

public class ACS_Brew : HeroAction
{
  public override string title => "Brew";
  public override string titlePresentProgressive => "Brewing";
  public override string description => "Concoct alcohol or potions.";
  public override PortraitZone location => Camp.zoneSupplies;

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
