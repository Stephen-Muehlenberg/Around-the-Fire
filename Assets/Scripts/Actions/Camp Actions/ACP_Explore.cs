using System;
using System.Collections;
using UnityEngine;

public class ACP_Explore : HeroAction
{
  public override string title => "Explore";
  public override string titlePresentProgressive => "Exploring";
  public override string description => "Scout the area for threats and opportunities.";
  public override HeroLocation location => HeroLocation.Perimeter;

  public override IEnumerator Process(Hero hero, PartyState previousState, PartyState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.REST, -15));
    HeroStatsPanel.ShowStatsFor(hero);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
