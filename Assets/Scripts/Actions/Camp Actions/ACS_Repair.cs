using System;
using System.Collections;
using UnityEngine;

public class ACS_Repair : HeroAction
{
  public override string title => "Repair";
  public override string titlePresentProgressive => "Repairing";
  public override string description => "Improve the condition of your weapons and armour.";
  public override HeroLocation location => HeroLocation.Supplies;

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
