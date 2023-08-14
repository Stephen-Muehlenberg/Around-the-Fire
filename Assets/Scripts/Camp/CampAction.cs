using System.Collections.Generic;

using HeroCampInfo = CampScene.HeroCampInfo;

public abstract class CampAction : HeroAction
{
  /// <summary>
  /// Location heroes doing this task will move to.
  /// </summary>
  public abstract CampScene.Location location { get; }

  /// <summary>
  /// Is this Action selectable by the Hero in the current context?
  /// </summary>
  public virtual Availability AvailableFor(HeroCampInfo hero, List<HeroCampInfo> party, GameState state) => Availability.AVAILABLE;
}
