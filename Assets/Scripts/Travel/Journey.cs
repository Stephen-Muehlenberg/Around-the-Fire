using System;

[Serializable]
public class Journey
{
  public static float EXPECTED_KM_PER_DAY = 32;

  public bool townIsDestination; // TODO Replace this with some kind of public Location destination field.
  public float distanceKm;
  public WorldTime startTime;
  public float kilometresTravelled;
  public float hoursTravelled;
  public float fractionComplete => kilometresTravelled / distanceKm;
  public float estimatedDurationDays => distanceKm / EXPECTED_KM_PER_DAY;
}
