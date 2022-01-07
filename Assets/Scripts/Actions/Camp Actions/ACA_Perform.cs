using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACA_Perform : HeroAction
{
  public override string title => "Perform";
  public override string titlePresentProgressive => "Performing";
  public override string description => "Entertain your friends around the fire.";

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.REST, -10));
    currentState.heroes
      .Where(it => it != hero)
      .ToList()
      .ForEach(it => RaiseStatsAndShowPopups(it, (Hero.Stat.MORALE, 20)));
    currentState.heroes
      .Where(it => it.location == HeroLocation.Fire)
      .ToList()
      .ForEach(it => RaiseStatsAndShowPopups(it, (Hero.Stat.MORALE, 10)));
    HeroStatsPanel.ShowStatsFor(hero);

    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
