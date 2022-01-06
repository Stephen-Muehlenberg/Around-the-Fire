using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampState
{
  public List<Hero> heroes = new List<Hero>();
  public FireState fire = FireState.NONE;
  public int firewood = 100; // TODO set to 0
  // TODO other properties.

  public enum FireState { NONE, SMALL, MEDIUM, LARGE }
}
