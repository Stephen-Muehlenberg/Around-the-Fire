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

  /// <summary>Average value of the hero's health, hunger, mood, and rest. Ranges from 0 to 100.</summary>
  public float totalSkill => (health + hunger + mood + rest) / 4f;

  /// <summary>
  /// Hero attempts a skill check, comparing their <see cref="totalSkill"/>
  /// against a d100 roll. Returns true on a success.
  /// </summary>
  /// <param name="difficultyModifier">Optional modifier to influence the difficulty. E.g. 10 = 10% harder.</param>
  public bool DoSkillCheck(int difficultyModifier = 0)
    => UnityEngine.Random.Range(0f, 100f) < (totalSkill - difficultyModifier);

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
  /// <summary>
  /// Hero starts suffering penalties to <see cref="rest"/> for every hour they're
  /// awake while sleepy. Value is based on <see cref="hoursAwake"/> and the hero's
  /// physical condition.
  /// </summary>
  // Ranges from 10 hours to 18 hours, depending on general health and fatigue.
  public bool sleepy => hoursAwake >= 10 + ((_health + _rest) / 200);

  public HeroAction action;
  public PortraitZone location;

  public enum Stat
  {
    HEALTH, HUNGER, REST, MORALE
  }

  public void SetAction(HeroAction action, bool assignedBySelf = false)
  {
    this.action = action;
  //  portrait.ShowSelectedAction(action);
  //  CampScene.OnActionSelected(this, assignedBySelf);
  }
}
