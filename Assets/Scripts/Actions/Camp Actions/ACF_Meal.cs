using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ACF_Meal : HeroAction
{
  public override string title => "Cook a Meal";
  public override string titlePresentProgressive => "Cooking a meal";
  public override string description => "Feed the whole party.";
  public override int hours => 2;

  public override bool AvailableFor(Hero hero, CampState campState)
  {
    return campState.fire > 0;
  }

  public override string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Come get your grub!",
      "Dinner is served.",
      "Get it while it's hot!",
      "Soup's on, everyone!",
    }.Random();
  }

  public override IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    RaiseStatsAndShowPopups(hero, (Hero.Stat.HUNGER, 45), (Hero.Stat.REST, -20));
    currentState.heroes
      .Where(it => it != hero)
      .ToList()
      .ForEach(it => RaiseStatsAndShowPopups(it, (Hero.Stat.HUNGER, 45)));
    // TODO Lower supplies.
    yield return new WaitForSeconds(1.5f);
    callback.Invoke();
  }
}
