using System.Collections.Generic;
using UnityEngine;

public class Hero
{
  public string name;
  public Sprite icon;

  /// <summary>0 - 100 inclusive.</summary>
  public float hunger;
  /// <summary>0 - 100 inclusive.</summary>
  public float rest;
  /// <summary>0 - 100 inclusive.</summary>
  public float mood;
  /// <summary>0 - 100 inclusive.</summary>
  public float health;

  public HeroAction action;
  public HeroPortrait portrait;
  public HeroLocation location;

  public enum Stat
  {
    HEALTH, HUNGER, REST, MORALE
  }
}
