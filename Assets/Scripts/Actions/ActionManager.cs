using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Loads an instance of every <see cref="HeroAction"/> into a list for
/// the game to consume. Should only need to be used once at game startup.
/// Hacky, but a fairly safe and mild hack.
/// </summary>
/// TODO See if we can somehow list HeroAction classes (*not* instances) in
/// the inspector, or load them dynamically.
public class ActionManager
{
  private static List<HeroAction> allActions;
  private static Dictionary<HeroLocation, List<HeroAction>> actionsByLocation;

  /// <summary>
  /// Must be called before any other <see cref="ActionManager"/> methods,
  /// and after HeroLocations are initialised (Start should be fine for this).
  /// </summary>
  public static void Initialise()
  {
    allActions = new List<HeroAction>()
    {
      new ACA_Rest(),
      new ACA_Talk(),
      new ACA_Carouse(),
      new ACA_Perform(),
      new ACC_Exercise(),
      new ACC_Train(),
      new ACC_Spar(),
      new ACC_Drill(),
      new ACF_CookingFire(),
      new ACF_Campfire(),
      new ACF_Bonfire(),
      new ACF_Meal(),
      new ACF_Feast(),
      new ACP_Wood(),
      new ACP_Forage(),
      new ACP_Explore(),
      new ACP_Guard(),
      new ACS_Snack(),
      new ACS_Ration(),
      new ACS_Repair(),
      new ACS_Brew(),
      new ACT_Heal(),
      new ACT_ReceiveHealing(),
      new ACT_Sleep(),
    };

    actionsByLocation = allActions
      .GroupBy(it => it.location)
      .ToDictionary(it => it.Key, it => it.ToList());
  }

  public static List<HeroAction> GetActionsFor(HeroLocation location)
    => actionsByLocation[location];

  /// <summary>
  /// Calculate which available Action the <paramref name="hero"/> most
  /// wants to assign themselves, and how much (0 min, 1 max) they want it.
  /// </summary>
  public static (HeroAction, float) GetMostWantedAction(Hero hero, CampState state)
  {
    return allActions
      .Where(it => it.location.HasSpace())
      .Where(it => it.AvailableFor(hero, state) == HeroAction.Availability.AVAILABLE)
      .Select(it => (it, it.GetAutoAssignWeight(hero, state)))
      .OrderByDescending(it => it.Item2)
      .First();
  }
}
