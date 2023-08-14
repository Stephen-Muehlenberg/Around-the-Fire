using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CampAction : HeroAction
{
  public abstract CampScene.Location location { get; }
}
