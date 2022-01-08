using System.Collections.Generic;

public static class Extensions
{
  public static T Random<T>(this List<T> list)
  {
    return list[UnityEngine.Random.Range(0, list.Count)];
  }

  public static T Random<T>(this T[] list)
  {
    return list[UnityEngine.Random.Range(0, list.Length)];
  }
}