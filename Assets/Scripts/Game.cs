using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Provides public static references to game state info.
/// Basically a collection of universal shortcuts.
/// </summary>
/// Note: should contain NO game logic. This is just state.
public static class Game
{
  public static GameState state { get; private set; }
  public static Party party => state.party;
  public static List<Hero> heroes => state.party.heroes;
  public static World world => state.world;
  public static WorldTime time => state.world.time;
  public static Journey journey => state.journey;
  public static Camp camp => state.camp;
  public static Settings settings { get; private set; }

  public static Action<WorldTime> onTimeChanged;
  public static Action<GameState> onCampaignStateChanged;
  public static Action<Settings> onSettingsChanged;

  public static void SetState(GameState state)
  {
    Game.state = state;
  }
}

/// <summary>
/// All information about the current game status.
/// </summary>
[Serializable]
public class GameState
{
  public Party party;
  /// <summary>Details about the party's current journey. Null if not currently travelling.</summary>
  public Journey journey;
  /// <summary>Details about the state of the camp. Null if not currently camping.</summary>
  public Camp camp;
  public World world;

  public GameState DeepCopy()
  {
    // TODO
    //    var a = JsonUtility.
    return new GameState();
  }
}

[Serializable]
public class Party
{
  public List<Hero> heroes = new();
  public Inventory inventory;
  // TODO Party current location.
  /// <summary>Details about the party's current quest, or null if no quest currently.</summary>
  public Quest quest;
}

[Serializable]
public class Inventory
{
  private int _money;
  public int money {
    get => _money; 
    set {
      _money = value;
      onInventoryChanged?.Invoke(this);
    }
  }

  private int _foodFresh;
  public int foodFresh
  {
    get => _foodFresh;
    set
    {
      _foodFresh = value;
      onInventoryChanged?.Invoke(this);
    }
  }

  private int _foodCured;
  public int foodCured
  {
    get => _foodCured;
    set
    {
      _foodCured = value;
      onInventoryChanged?.Invoke(this);
    }
  }

  /// <summary>
  /// Total amount of food (<see cref="foodFresh"/> + <see cref="foodCured"/>).
  /// 
  /// </summary>
  public int food => _foodFresh + _foodCured;

  private int _firewood;
  public int firewood
  {
    get => _firewood;
    set
    {
      _firewood = value;
      onInventoryChanged?.Invoke(this);
    }
  }

  public float daysWorthOfSupplies(Party forParty)
    => food / 4f / forParty.heroes.Count;

  public float daysWorthOfFirewood(Party forParty)
    => firewood / 8 / forParty.heroes.Count;

  /// <summary>
  /// Removes the specified <paramref name="amount"/> of food, starting from <see cref="foodFresh"/>,
  /// then from <see cref="foodCured"/> if fresh runs out. Throws an exception if not enough
  /// food.
  /// </summary>
  public void consumeFood(int amount)
  {
    if (amount > food)
      throw new Exception("Tried to consume " + amount + " food but only have " + food + " total.");

    if (amount <= _foodFresh)
      _foodFresh -= amount;
    else
    {
      amount -= _foodFresh;
      _foodFresh = 0;
      _foodCured -= amount;
    }
    onInventoryChanged?.Invoke(this);
  }

  public UnityAction<Inventory> onInventoryChanged;
}

/// <summary>
/// Info about the state of the game world, NPCs, etc.
/// </summary>
[Serializable]
public class World
{
  public WorldTime time;
  // Towns
  // NPCs
}

[Serializable]
public class WorldTime
{
  /// <summary>Number of days since campaign start. First day is day 0.</summary>
  public int day;
  /// <summary>Clamped between 0 and 23.999~. 0 = midnight, 12 = noon, etc.</summary>
  public float hourOfDay;

  public bool isDaytime => hourOfDay >= 6 && hourOfDay < 19;
  public string timeOfDayDescription { get {
      if (hourOfDay < 5) return "Late Night";
      if (hourOfDay < 7) return "Early Morning";
      if (hourOfDay < 9) return "Morning";
      if (hourOfDay < 11) return "Late Morning";
      if (hourOfDay < 13) return "Noon";
      if (hourOfDay < 15) return "Mid Afternoon";
      if (hourOfDay < 17) return "Late Afternoon";
      if (hourOfDay < 19) return "Sunset";
      if (hourOfDay < 21) return "Evening";
      return "Night";
    } }

  public void Advance(float hours, bool updateRest = true)
  {
    if (hours < 0) throw new Exception("Time must be advanced by a positive number but was " + hours + ".");
    if (hours == 0) return;

    float previousHours = (day * 24) + hourOfDay;
    float newHours = previousHours + hours;
    day = Mathf.FloorToInt(newHours / 24f);
    hourOfDay = newHours % 24;

    // TODO The following should not be here. Move it elsewhere.

    // Track how long heroes have been awake.
    Game.heroes.ForEach(it => {
      it.hoursAwake += hours;

      // If they've been awake over 12 hours, start getting sleepy.
      if (updateRest && it.hoursAwake > 12)
      {
        int hoursAfter12 = (int) it.hoursAwake - 12;
        float hoursAdvancedAfter12 = Mathf.Min(hoursAfter12, hours);
        it.rest -= 5 * hoursAdvancedAfter12;
      }
    });
  }

  public WorldTime Copy() => new WorldTime() { day = day, hourOfDay = hourOfDay };
}

[Serializable]
public class Settings
{
  // player setting, e.g. volume
}
