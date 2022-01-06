using System.Collections.Generic;

/// <summary>
/// Loads an instance of every <see cref="HeroAction"/> into a list for
/// the game to consume. Should only need to be used once at game startup.
/// Hacky, but a fairly safe and mild hack.
/// </summary>
/// TODO See if we can somehow list HeroAction classes (*not* instances) in
/// the inspector, or load them dynamically.
public class ActionManager
{
  public static List<HeroAction> GetActionsFor(HeroLocation location)
  {
    if (location == HeroLocation.Fire) return new List<HeroAction>() {
      new ACF_CookingFire(),
      new ACF_Campfire(),
      new ACF_Bonfire(),
      new ACF_Meal(),
      new ACF_Feast(),
    };
    if (location == HeroLocation.Around) return new List<HeroAction>() {
      new ACA_Rest(),
      new ACA_Talk(),
      new ACA_Carouse(),
      new ACA_Perform(),
    };
    if (location == HeroLocation.Tent) return new List<HeroAction>() {
      new ACT_Sleep(),
    };
    if (location == HeroLocation.Supplies) return new List<HeroAction>() {
      new ACS_Ration(),
    };
    if (location == HeroLocation.Clearing) return new List<HeroAction>() {
      new ACC_Train(),
    };
    if (location == HeroLocation.Perimeter) return new List<HeroAction>() {
    };

    return new List<HeroAction>();
  }
}
