using System;
using UnityEngine;

[Serializable]
public class Hero : Character
{
  public enum Gender { Male, Female }
  public Gender gender;
  public string heShe => gender == Gender.Male ? "he" : "she";
  public string himHer => gender == Gender.Male ? "him" : "her";
  public string hisHer => gender == Gender.Male ? "his" : "her";

  public float totalSkill => health + hunger + mood + rest;

  /// <summary>0 - 100 inclusive.</summary>
  public float health {
    get => _health;
    set {
      float clamped = Mathf.Clamp(value, 0, 100);
      if (_health == clamped) return;
      _health = clamped;
      statusChanges?.Invoke(this);
  } }
  private float _health;
  /// <summary>0 - 100 inclusive.</summary>
  public float hunger
  {
    get => _hunger;
    set
    {
      float clamped = Mathf.Clamp(value, 0, 100);
      if (_hunger == clamped) return;
      _hunger = clamped;
      statusChanges?.Invoke(this);
    }
  }
  private float _hunger;
  /// <summary>0 - 100 inclusive.</summary>
  public float mood
  {
    get => _mood;
    set
    {
      float clamped = Mathf.Clamp(value, 0, 100);
      if (_mood == clamped) return;
      _mood = clamped;
      statusChanges?.Invoke(this);
    }
  }
  private float _mood;
  /// <summary>0 - 100 inclusive.</summary>
  public float rest
  {
    get => _rest;
    set
    {
      float clamped = Mathf.Clamp(value, 0, 100);
      if (_rest == clamped) return;
      _rest = clamped;
      statusChanges?.Invoke(this);
    }
  }
  private float _rest;

  public event Action<Hero> statusChanges;

  public float hoursAwake;

  public HeroAction action;
  public PortraitZone location;

  public enum Stat
  {
    HEALTH, HUNGER, REST, MORALE
  }

  // TODO how does the hour's activities affect this? combat? travel? sleep?
  // rest? play?
  public void UpdateStatsAtEndOfHour()
  {
    this.rest -= 6;
  }

  public void SelectAction(HeroAction action, bool assignedBySelf = false)
  {
    this.action = action;
  //  portrait.ShowSelectedAction(action);
  //  CampScene.OnActionSelected(this, assignedBySelf);
  }
}
