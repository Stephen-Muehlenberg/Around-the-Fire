using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls scene lighting based on the specified time of day.
/// </summary>
public class TimeOfDayController : MonoBehaviour
{
  private static readonly float ADVANCE_SPEED_SECONDS_PER_HOUR = 2.5f;

  [SerializeField] private bool demoOnStart;
  [SerializeField] private Light sun;
  [SerializeField] private Camera mainCamera;
  [SerializeField] private Vector3[] sunDirectionByHour;

  [Tooltip("Main light color. Each Gradient covers a 6 hour period, starting from midnight. Alpha is intensity.")]
  [SerializeField] private Gradient[] sunColor;

  [Tooltip("Background color. Each Gradient covers a 6 hour period, starting from midnight.")]
  [SerializeField] private Gradient[] skyColor;

  private void Start()
  {
    if (sunColor.Length != 4 || skyColor.Length != 4)
      throw new Exception("sunColor[] and skyColor[] must have exactly 4 elements, but had " + sunColor.Length + " and " + skyColor.Length + ", respectively.");
    if (demoOnStart)
      PlayDemo(0, 0);
  }

  /// <summary>
  /// Continuously advances time in a loop, to show off how it looks.
  /// </summary>
  private void PlayDemo(float _, float __)
  {
    AdvanceTime(0, 24, null, PlayDemo);
  }

  /// <summary>
  /// Animate the scene lighting changing over time.
  /// <paramref name="startHour"/> is in 24hour time, and 
  /// the start and end times must be within <see cref="MIN_HOUR"/>
  /// and <see cref="MAX_HOUR"/>.
  /// </summary>
  public void AdvanceTime(
    float startTime, int hours,
    Action<int> onHourEnd = null, Action<float, float> onFinished = null)
  {
    if (hours <= 0) throw new Exception("Time must be between 1 and 24 but was " + hours + ".");
    StartCoroutine(AdvanceTimeCoroutine(startTime, hours, onHourEnd, onFinished));
  }

  private IEnumerator AdvanceTimeCoroutine(
    float startTime, int hours, 
    Action<int> onHourEnd, Action<float, float> onFinish)
  {
    float endTime = startTime + hours;
    if (endTime > 24) endTime -= 24;

    float progress = 0; // Fraction of total progress, 0 = start, 1 = end.
    float currentTime = startTime; // World time in decimal, e.g. 2.5 = 2:30am.
    int currentHourIndex;
    int previousHourIndex = (int) startTime;

    // Advance time and update lighting in a loop.
    while (currentTime < endTime)
    {
      currentHourIndex = (int) currentTime;

      // Invoke callback when we pass an hour mark.
      if (onHourEnd != null && previousHourIndex < currentHourIndex)
      {
        previousHourIndex++;
        onHourEnd.Invoke(currentHourIndex);
      }

      // Update visuals.
      SetTime(currentTime);

      // Advance time, then smooth step it for the purposes of animation.
      progress += Time.deltaTime / ADVANCE_SPEED_SECONDS_PER_HOUR / hours;
      currentTime = startTime + (Mathf.SmoothStep(0, hours, progress) * hours);

      yield return null;
    }

    // Snap to exactly the end time.
    SetTime(endTime);

    // Invoke final callback.
    if (onFinish != null) onFinish.Invoke(hours, endTime);
  }

  /// <summary>
  /// Sets scene lighting to match the specified time of day.
  /// <paramref name="hour"/> must be in 24-hour time between (0 - 23.999...).
  /// </summary>
  public void SetTime(float time)
  {
    if (time < 0 || time >= 24) throw new Exception("Time must be between 0 (inclusive) and 24 (exclusive), but was " + time + ".");

    // Which quarter of the day (0-6, 6-12, 12-18, 18-24) are we in?
    int dayQuarter = Mathf.FloorToInt(time / 6);
    // How far through that quater are we?
    float quarterFraction = (time - (dayQuarter * 6)) / 6f;

    // Set light and background colours.
    sun.color = sunColor[dayQuarter].Evaluate(quarterFraction);
    sun.intensity = sun.color.a;
    mainCamera.backgroundColor = skyColor[dayQuarter].Evaluate(quarterFraction);

    // For all times except the final moment, Lerp between different sun angles.
/*    if (currentHourIndex + 1 < sunDirectionByHour.Length)
      sun.transform.rotation = Quaternion.Euler(Vector3.Lerp(
        a: sunDirectionByHour[currentHourIndex],
        b: sunDirectionByHour[currentHourIndex + 1],
        t: currentHourFraction));
    // For the very final moment (and end of the array), just use the last sun angle.
    else
      sun.transform.rotation = Quaternion.Euler(sunDirectionByHour[currentHourIndex]);*/
  }
}
