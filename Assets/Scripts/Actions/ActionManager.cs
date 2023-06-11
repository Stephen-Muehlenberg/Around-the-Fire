using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Loads an instance of every <see cref="HeroAction"/> into a list for
/// the game to consume. Should only need to be used once at game startup.
/// Hacky, but a fairly safe and mild hack.
/// </summary>
/// TODO See if we can somehow list HeroAction classes (*not* instances) in
/// the inspector, or load them dynamically.
public class ActionManager : MonoBehaviour
{
  private static ActionManager singleton;

  private List<HeroAction> travelActions;
  private List<HeroAction> campActions;
  private Dictionary<PortraitZone, List<HeroAction>> campActionsByLocation;

  public void Awake()
  {
    if (singleton != null)
    {
      Destroy(gameObject);
      return;
    }

    singleton = this;
    DontDestroyOnLoad(this.gameObject);

    travelActions = new List<HeroAction>()
    {
      new ATC_March(),
      new ATC_Relax(),
      new ATC_Guard(),
      new ATC_Forage(),
    };

    campActions = new List<HeroAction>()
    {
      new AC2A_Relax(),
      new AC2C_Train(),
      new AC2F_BuildFire(),
      new AC2F_HotMeal(),
      new AC2P_GatherWood(),
      new AC2S_ColdMeal(),
      new AC2T_Sleep(),
    //  new ACA_Talk(),
    //  new ACA_Carouse(),
    //  new ACA_Perform(),
    //  new ACC_Exercise(),
    //  new ACC_Spar(),
    //  new ACC_Drill(),
    //  new ACF_CookingFire(),
    //  new ACF_Campfire(),
    //  new ACF_Bonfire(),
    //  new ACF_Meal(),
    //  new ACF_Feast(),
    //  new ACP_Forage(),
    //  new ACP_Explore(),
    //  new ACP_Guard(),
    //  new ACS_Snack(),
    //  new ACS_Ration(),
    //  new ACS_Repair(),
    //  new ACS_Brew(),
    //  new ACT_Heal(),
    //  new ACT_ReceiveHealing(),
    };

    campActionsByLocation = campActions
      .GroupBy(it => it.location)
      .ToDictionary(it => it.Key, it => it.ToList());
  }

  public static List<HeroAction> GetTravelActionsFor(Hero hero, GameState context)
  {
    // TODO Filter out unavailable actions.
    return singleton.travelActions;
  }

  /// <summary>
  /// Calculate which available Action the <paramref name="hero"/> most
  /// wants to assign themselves, and how much (0 min, 1 max) they want it.
  /// </summary>
  public static (HeroAction, float) GetMostWantedTravelAction(Hero hero, GameState context)
  {
    return (singleton.travelActions.Random(), 0f); // TODO remove this, use below.
    /*    return travelActions
          .Where(it => it.AvailableFor(hero, context) == HeroAction.Availability.AVAILABLE)
          .Select(it => (it, it.GetAutoAssignWeight(hero, context)))
          .OrderByDescending(it => it.Item2)
          .First();*/
  }

  public static List<HeroAction> GetCampActionsFor(PortraitZone location)
  {
    if (singleton.campActionsByLocation.ContainsKey(location))
      return singleton.campActionsByLocation[location];
    return new List<HeroAction>();
  }

  /// <summary>
  /// Calculate which available Action the <paramref name="hero"/> most
  /// wants to assign themselves, and how much (0 min, 1 max) they want it.
  /// </summary>
  public static (HeroAction, float) GetMostWantedCampAction(Hero hero, GameState state)
  {
    return singleton.campActions
      .Where(it => it.location.hasSpace)
      .Where(it => it.AvailableFor(hero, state) == HeroAction.Availability.AVAILABLE)
      .Select(it => (it, it.GetAutoAssignWeight(hero, state)))
      .OrderByDescending(it => it.Item2)
      .First();
  }

  public static HeroAction GetDefaultCampAction()
  {
    return singleton.campActions.First(it => it is AC2A_Relax);
  }
}
