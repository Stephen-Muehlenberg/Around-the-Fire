using System;
using System.Collections;
using UnityEngine;

public class ACS_Brew : HeroAction
{
  public override string title => "Brew";
  public override string titlePresentProgressive => "Brewing";
  public override string description => "Concoct alcohol or potions.";
  public override HeroLocation location => HeroLocation.Supplies;

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
