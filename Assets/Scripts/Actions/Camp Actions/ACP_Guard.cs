using System;
using System.Collections;
using UnityEngine;

public class ACP_Guard : HeroAction
{
  public override string title => "Stand Watch";
  public override string titlePresentProgressive => "Standing watch";
  public override string description => "Guard the camp.";

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.REST, -10));
    hero.portrait.Select();

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
