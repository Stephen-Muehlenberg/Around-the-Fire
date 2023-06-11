using System;
using UnityEngine;

[Serializable]
public class Journey
{
  public static float EXPECTED_KM_PER_DAY = 32;

  public Location destination;
  public float distanceKm;
  public WorldTime startTime;
  public float kilometresTravelled;
  /// <summary>Number of days spent travelling, rounded up (e.g. the first day of travel is day 1, the second is day 2).</summary>
  public int dayOfTravel => 1 + Game.time.day - startTime.day;
  public float fractionComplete => kilometresTravelled / distanceKm;
  public int estimatedDurationDays => Mathf.CeilToInt(distanceKm / EXPECTED_KM_PER_DAY);
}

// TODO Move this to its own file, add more properties.
public class Location
{
  public string name;
  public bool isTown; // TODO replace this with a more in-depth system.
}