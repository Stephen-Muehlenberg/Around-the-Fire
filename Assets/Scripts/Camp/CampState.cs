using System.Collections.Generic;

public class CampState
{
  public FireState fire = FireState.NONE;
  // TODO other properties.

  public enum FireState { NONE, SMALL, MEDIUM, LARGE }
}
