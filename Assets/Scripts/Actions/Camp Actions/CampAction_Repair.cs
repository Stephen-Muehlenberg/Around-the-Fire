using System;
using System.Collections;
using UnityEngine;

public class CampAction_Repair : CampAction
{
  public override string title => "Repair";
  public override string titlePresentProgressive => "Repairing";
  public override string description => "Improve the condition of your weapons and armour.";
  public override CampScene.Location location => CampScene.Location.Supplies;

  public override IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback)
  {
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
