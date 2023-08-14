using System.Collections.Generic;
using System.Linq;

using Availability = HeroAction.Availability;
using HeroCampInfo = CampScene.HeroCampInfo;

public class CampActionManager
{
  private readonly List<CampAction> actions = new() {
      new CampAction_Relax2(),
      new CampAction_Train2(),
      new CampAction_BuildFire2(),
      new CampAction_HotMeal2(),
      new CampAction_GatherWood2(),
      new CampAction_ColdMeal2(),
      new CampAction_Sleep2(),
  };

  public CampAction GetDefault() => actions[0];

  public List<(CampAction, Availability)> GetAllAvailable(HeroCampInfo hero, List<HeroCampInfo> party, GameState state)
    => actions
      .Select(it => (it, it.AvailableFor(hero, party, state)))
      .Where(it => it.Item2 != Availability.HIDDEN)
      .ToList();

  /// <summary>
  /// Calculate which available action the <paramref name="hero"/> most
  /// wants to assign themselves, and how much (0 min, 1 max) they want it.
  /// </summary>
  public (CampAction, float) GetMostWanted(HeroCampInfo hero, List<HeroCampInfo> party, GameState state)
    => actions
    .Where(it => it.AvailableFor(hero, party, state) == Availability.AVAILABLE)
    .Select(it => (it, it.GetAutoAssignWeight(hero.hero, state)))
    .OrderByDescending(it => it.Item2)
    .First();

  /// <summary>
  /// Calculate the top 3 available actions the <paramref name="hero"/> most
  /// wants to assign themselves, and how much (0 min, 1 max) they want it.
  /// </summary>
  public List<(CampAction, float)> GetTop3(HeroCampInfo hero, List<HeroCampInfo> party, GameState state)
    => actions
    .Where(it => it.AvailableFor(hero, party, state) == Availability.AVAILABLE)
    .Select(it => (it, it.GetAutoAssignWeight(hero.hero, state)))
    .OrderByDescending(it => it.Item2)
    .Take(3)
    .ToList();
}
