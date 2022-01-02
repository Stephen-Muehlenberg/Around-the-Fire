using System;
using UnityEngine;

[CreateAssetMenu]
public class CampAction : ScriptableObject
{
  [Serializable]
  public struct Property
  {
    public string key;
    public int value;
  }

  public string title;
  public string titlePresentProgressive;
  public string description;
  public int hours;
  public bool partnersReduceHours;
  public int requiredPartners;
  public string[] completionAnnouncements;
  public Property[] properties;
}
