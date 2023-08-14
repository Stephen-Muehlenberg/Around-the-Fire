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
      new TravelAction_March(),
      new TravelAction_Relax(),
      new TravelAction_Guard(),
      new TravelAction_Forage(),
    };
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
}
