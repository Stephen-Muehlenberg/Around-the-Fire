using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Public static wrapper for party state, for ease of access.
/// </summary>
public static class Party
{
  public static PartyState currentState;

  public static float timeOfDay => currentState.timeOfDay;
  public static bool isDaytime => currentState.timeOfDay >= 6 && currentState.timeOfDay < 19;
  public static List<Hero> heroes => currentState.heroes;
  public static float supplies => currentState.supplies;
  public static float firewood => currentState.firewood;
  public static JourneyState journey => currentState.journey;
  public static CampState camp => currentState.camp;

  public static void AdvanceTime(float hours, bool updateRest = true)
  {
    if (hours < 0) throw new Exception("Time must be advanced by a positive number but was " + hours + ".");
    if (hours == 0) return;

    float previousTime = currentState.totalTime;
    currentState.totalTime += hours;

    // Track how long heroes have been awake.
    int previousHour = (int) previousTime;
    int newHour = (int) currentState.totalTime;
    int hoursAdvanced = newHour - previousHour;
    if (hoursAdvanced < 1) return;
    heroes.ForEach(it => {
      it.hoursAwake += hoursAdvanced;

      // If they've been awake over 12 hours, start getting sleepy.
      if (updateRest && it.hoursAwake > 12)
      {
        int hoursAfter12 = (int) it.hoursAwake - 12;
        int hoursAdvancedAfter12 = Mathf.Min(hoursAfter12, hoursAdvanced);
        it.rest -= 5 * hoursAdvancedAfter12;
      }
    });
  }
}

/// <summary>
/// Core game state information.
/// </summary>
public class PartyState
{
  public float timeOfDay => totalTime % 24;
  public int day => (int) (totalTime / 24);
  public List<Hero> heroes = new List<Hero>();
  /// <summary>Details about the party's current journey. Null if not travelling anywhere.</summary>
  public JourneyState journey;
  /// <summary>Details about the state of the camp. Null if no camp currently set up.</summary>
  public CampState camp;

  /// <summary>Invoked when certain properties are modified.</summary>
  public event Action<PartyState> updates;

  /// <summary>Total number of hours.</summary>
  public float totalTime {
    get => _totalTime;
    set {
      if (_totalTime == value) return;
      _totalTime = value;
      updates?.Invoke(this);
  } }
  private float _totalTime;

  public float supplies {
    get => _supplies;
    set {
      if (_supplies == value) return;
      _supplies = value;
      updates?.Invoke(this);
  } }
  private float _supplies;

  public float firewood {
    get => _firewood;
    set {
      if (_firewood == value) return;
      _firewood = value;
      updates?.Invoke(this);
  } }
  private float _firewood;

  /// <summary>
  /// Returns a deep copy of this <see cref="PartyState"/>.
  /// </summary>
  public PartyState Clone()
  {
    var state = new PartyState()
    {
      totalTime = Party.currentState.totalTime,
      heroes = new List<Hero>(Party.heroes.Count),
      supplies = Party.supplies,
      firewood = Party.firewood,
      journey = Party.journey, // TODO Might need to deep copy journey?
      camp = new CampState()
      {
        fire = Party.camp.fire,
      }
    };
    Party.heroes.ForEach(hero =>
    {
      state.heroes.Add(new Hero()
      {
        name = hero.name,
        icon = hero.icon,
        health = hero.health,
        hunger = hero.hunger,
        mood = hero.mood,
        rest = hero.rest,
        action = hero.action,
        location = hero.location
      });
    });
    return state;
  }
}
