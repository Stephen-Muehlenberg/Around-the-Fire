using System.Collections.Generic;
using UnityEngine;

public class Adventurer
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

  public CampAction action;
  public AdventurerPortrait portrait;

  public enum Stat
  {
    HEALTH, HUNGER, REST, MORALE
  }

  public ActionResult PerformAction(List<Adventurer> party) =>
    ActionResult.GetFor(action, this, party);
}
