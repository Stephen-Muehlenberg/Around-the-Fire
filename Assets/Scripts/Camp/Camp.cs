using System;
using System.Collections.Generic;

[Serializable]
public class Camp
{
  // These are static because the concept of different
  // Zones are independant of any Camp instance. Lets
  // actions, etc, refer to Zones even if no Camp exists.
  public static PortraitZone zoneFire = new() { name = "Fire", maxPortraits = 2 };
  public static PortraitZone zoneAround = new() { name = "Around", maxPortraits = 8 };
  public static PortraitZone zoneTent = new() { name = "Tent", maxPortraits = 8 };
  public static PortraitZone zoneSupplies = new() { name = "Supplies", maxPortraits = 2 };
  public static PortraitZone zoneClearing = new() { name = "Clearing", maxPortraits = 8 };
  public static PortraitZone zonePerimeter = new() { name = "Perimeter", maxPortraits = 8 };
  public static List<PortraitZone> zones = new() { zoneFire, zoneAround, zoneTent, zoneSupplies, zoneClearing, zonePerimeter };

  public FireState fire = FireState.NONE;
  // TODO other properties.

  public enum FireState { NONE, SMALL, MEDIUM, LARGE }

  public Camp()
  {
    // Reset zones when a new camp is set.
    zoneFire.portraits.Clear();
    zoneAround.portraits.Clear();
    zoneTent.portraits.Clear();
    zoneSupplies.portraits.Clear();
    zoneClearing.portraits.Clear();
    zonePerimeter.portraits.Clear();
  }
}

/*
 * "ZoneId"
 * There are Zones which are static, stateless concepts
 * Like, the concept of the Camp.Zone.Fire exists, even when there is no camp
 * This is used for Actions and so forth to identify where they go.
 * E.g. "I am available around the fire"
 * These are identifiers.
 * 
 * Ideally, you can access them via
 * Camp.Zones.Fire, or CampZone.FIRE
 * 
 * HOWEVER
 * Actions, as a concept, are not tied to a specific scene.
 * So actions can't have a CampZone instance. It would have to be an enum.
 * So these need to be either plain ints, or some generic class
 * Generic class would be better for ensuring type safety, and preventing
 * accidental reproduction of enum values.
 * But then, it's not like they need to be unique between scenes; just
 * unique within the scene.
 * Could have enum Zones { CampFIRE, CampPerimeter, TownFoo, TravelBlah }
 * This would lose the speed benefit of using an enum, though - arrays wouldn't
 * be starting at 0, so you'd have to do a lookup. A dictionary would be
 * basically the same cost.
 * What does that look like?
 * public class ZoneId {
 *   public int maxHeroes;
 * }
 * public static class CampZone {
 *   public static ZoneId Fire = new Zone() { maxHeroes = 2 };
 * }
 * 
 * 
 * "ZoneInstance"
 * There is also the concept of an actual instance of a camp with an actual
 * fire, and actual places around that fire.
 * This has properties, like heroes who are assigned to it.
 * 
 * Ideally, you access this via the Game directory
 * Game.camp.zone[CampZone.FIRE]
 * If this is just an array with an enum for a key, then it's super cheap to access.
 * 
 * Camp {
 *   public ZoneInstance[] zone = new() {
 *     
 *   }
 * }
 * 
 * Ok, with a dictionary it would look like
 * Camp {
 *   public Dictionary<ZoneId, ZoneInstance> zone = new() {
 *     { CampZone.Fire, new ZoneInstance() { ... },
 *   }
 * }
 * and then you'd access it like
 * Game.camp.zone[CampZone.Fire].heroes ...
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * V3
 * Ok, what if it's just static
 * Camp.zone.fire
 * Camp {
 *   public static Zone fire = new() { maxHeroes = 2 };
 * }
 * we would have to reset it when a new Camp is created, but that's not so bad
 * it can be done automatically on OnCreate() or whatever
 * public Camp() {
 *   Camp.zone.fire.heroes.clear();
 *   Camp.zone.around.heroes.clear();
 * }
 */