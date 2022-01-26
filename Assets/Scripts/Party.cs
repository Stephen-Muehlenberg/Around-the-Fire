using System.Collections.Generic;

/// <summary>
/// Public static wrapper for party state, for ease of access.
/// </summary>
public class Party
{
  public static PartyState currentState;

  public static float time => currentState.time;
  public static List<Hero> heroes => currentState.heroes;
  public static float supplies => currentState.supplies;
  public static float firewood => currentState.firewood;
  public static JourneyState journey => currentState.journey;
  public static CampState camp => currentState.camp;
}

/// <summary>
/// Core game state information.
/// </summary>
public class PartyState
{
  /// <summary>24-hour time, e.g. 0 = 12am, 1 = 1am, 13 = 1pm.</summary>
  public float time;
  public List<Hero> heroes = new List<Hero>();
  public float supplies;
  public float firewood;
  /// <summary>Details about the party's current journey. Null if not travelling anywhere.</summary>
  public JourneyState journey;
  /// <summary>Details about the state of the camp. Null if no camp currently set up.</summary>
  public CampState camp;
}
