using System;

[Serializable]
public class Quest
{
  public string title;
  public string description;
  public int rewardMoney;
  public float distanceFromTownKm;
  public bool complete;
  // quest type, origin, time limit
}
