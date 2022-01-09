using System;
using System.Collections;
using UnityEngine;

public class ACS_Brew : HeroAction
{
  public override string title => "Brew";
  public override string titlePresentProgressive => "Brewing";
  public override string description => "Concoct alcohol or potions.";
  public override HeroLocation location => HeroLocation.Supplies;

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.REST, -5));
    HeroStatsPanel.ShowStatsFor(hero);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
