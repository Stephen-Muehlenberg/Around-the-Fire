using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains various useful extension methods.
/// </summary>
public static class Extensions
{
  /// <summary>
  /// Starts a <see cref="Coroutine"/> which waits for <paramref name="delay"/> seconds then
  /// invokes <paramref name="action"/>.
  /// </summary>
  public static Coroutine DoAfterDelay(this MonoBehaviour self, float delay, Action action)
  {
    return self.StartCoroutine(DoAfterDelayCoroutine(delay, action));
  }

  /// <summary>
  /// Companion method of <see cref="DoAfterDelay(MonoBehaviour, float, Action)"/>.
  /// </summary>
  private static IEnumerator DoAfterDelayCoroutine(float delay, Action action)
  {
    yield return new WaitForSeconds(delay);
    action.Invoke();
  }

  /// <summary>
  /// Starts a <see cref="Coroutine"/> which waits for <paramref name="delay"/> seconds then
  /// starts the provided <paramref name="coroutine"/>.
  /// </summary>
  public static Coroutine StartAfterDelay(this MonoBehaviour self, float delay, IEnumerator coroutine)
  {
    return self.StartCoroutine(StartAfterDelayCoroutine(self, delay, coroutine));
  }

  /// <summary>
  /// Companion method of <see cref="StartAfterDelay(MonoBehaviour, float, IEnumerator)"/>.
  /// </summary>
  private static IEnumerator StartAfterDelayCoroutine(MonoBehaviour self, float delay, IEnumerator coroutine)
  {
    yield return new WaitForSeconds(delay);
    self.StartCoroutine(coroutine);
  }

  /// <summary>
  /// Returns a random element from the <paramref name="list"/>.
  /// </summary>
  public static T Random<T>(this List<T> list)
  {
    return list[UnityEngine.Random.Range(0, list.Count)];
  }

  /// <summary>
  /// Returns a random element from the <paramref name="array"/>.
  /// </summary>
  public static T Random<T>(this T[] array)
  {
    return array[UnityEngine.Random.Range(0, array.Length)];
  }
}
