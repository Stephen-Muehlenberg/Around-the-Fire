using System;
using System.Collections.Generic;
using UnityEngine;

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
  /// <summary>Details about the party's current journey. Null if not travelling anywhere.</summary>
  /// TODO This perhaps oughtn't be part of the party info. Consider moving it.
  public Journey journey;
  /// <summary>Details about the state of the camp. Null if no camp currently set up.</summary>
  /// TODO This perhaps oughtn't be part of the party info. Consider moving it.
  public CampState camp;
}

[Serializable]
public class Inventory
{
  public int money;
  public float supplies;
  public float firewood;

  public float daysWorthOfSupplies(Party forParty)
    => supplies / 4 / forParty.heroes.Count;

  public float daysWorthOfFirewood(Party forParty)
    => firewood / 8 / forParty.heroes.Count;
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
public struct WorldTime
{
  /// <summary>Number of days since campaign start. First day is day 0.</summary>
  public int day;
  /// <summary>Clamped between 0 and 23.999~. 0 = midnight, 12 = noon, etc.</summary>
  public float hourOfDay;

  public bool isDaytime => hourOfDay >= 6 && hourOfDay < 19;

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
}

[Serializable]
public class Settings
{
  // player setting, e.g. volume
}
