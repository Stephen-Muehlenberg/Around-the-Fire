using System.Collections.Generic;

public class CampState
{
  public int hour;
  public List<Hero> heroes = new List<Hero>();
  public FireState fire = FireState.NONE;
  public float supplies;
  public float firewood;
  // TODO other properties.

  public enum FireState { NONE, SMALL, MEDIUM, LARGE }
}
