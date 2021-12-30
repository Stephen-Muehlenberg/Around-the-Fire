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
  public string shortDesc;
  public string fullDesc;
  public string[] completionAnnouncements;
  public Property[] properties;
}
