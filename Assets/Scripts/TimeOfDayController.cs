using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls scene lighting based on the specified time of day.
/// </summary>
public class TimeOfDayController : MonoBehaviour
{
  private const float ADVANCE_SPEED_SECONDS_PER_HOUR = 2.5f;

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
    if (hours <= 0) throw new Exception("hours must be be at least 1 but was " + hours + ".");
    StartCoroutine(AdvanceTimeCoroutine2(startTime, hours, onHourEnd, onFinished));
  }

  private IEnumerator AdvanceTimeCoroutine2(
    float startTime, int hours,
    Action<int> onHourEnd, Action<float, float> onFinish)
  {
    float elapsedTime = 0;
    float totalAnimationDuration = hours * ADVANCE_SPEED_SECONDS_PER_HOUR;

    float currentTime = startTime;
    int currentHourIndex = Mathf.FloorToInt(startTime);

    while (elapsedTime < totalAnimationDuration)
    {
      elapsedTime += Time.deltaTime;
      currentTime += Time.deltaTime / ADVANCE_SPEED_SECONDS_PER_HOUR;

      // Check if the previous hour has ended and we've started a new hour.
      if (Mathf.FloorToInt(currentTime) > currentHourIndex)
      {
        if (currentTime >= 24)
        {
          currentTime -= 24;
          currentHourIndex = 0;
        } else currentHourIndex++;

        onHourEnd?.Invoke(currentHourIndex);
      }

      SetTime(currentTime);

      yield return null;
    }

    float endTime = Mathf.Repeat(startTime + hours, 24);
    SetTime(endTime);
    onFinish?.Invoke(hours, endTime);
  }

  private IEnumerator AdvanceTimeCoroutine(
    float startTime, int hours, 
    Action<int> onHourEnd, Action<float, float> onFinish)
  {
    float hoursPassed = 0;
    float hoursPassedSmoothed;
    float progressFraction = 0;
    float currentTime = startTime; // Hour of day in decimal, e.g. 2.5 = 2:30am.
    int currentHourIndex = (int) startTime;
    int previousHourIndex = currentHourIndex;

    // Advance time and update lighting in a loop.
    while (progressFraction < 1f)
    {
      // Advance progressFraction linearly.
      hoursPassed += Time.deltaTime / ADVANCE_SPEED_SECONDS_PER_HOUR;
      progressFraction = hoursPassed / hours;

      // Then map progress to time of day, using SmoothStep for a smoother animation.
      hoursPassedSmoothed =
//      Mathf.SmoothStep(0, hours, progressFraction);
        hoursPassed;
      currentTime = Mathf.Repeat(
        startTime + hoursPassedSmoothed,
        24);
    //  Debug.Log((progressFraction * 100) + "%, hours " + hoursPassed + ", smoothed " + hoursPassedSmoothed + ", time " + currentTime);

      // Update visuals.
      SetTime(currentTime);

      // Check if we passed an hour mark.
      currentHourIndex = Mathf.FloorToInt(currentTime);
      if (currentHourIndex > previousHourIndex)
      {
        hoursPassed++;

        // Wrap back around to 0 (midnight) if we hit 24 hours.
        if (currentHourIndex == 24)
        {
          currentHourIndex = 0;
          previousHourIndex = 0;
        } else previousHourIndex++;

        onHourEnd?.Invoke(currentHourIndex);
      }

      yield return null;
    }

    // Snap to exactly the end time.
    float endTime = Mathf.Repeat(startTime + hours, 24);
    SetTime(endTime);

    // Invoke final callback.
    onFinish?.Invoke(hours, endTime);
  }

  // Cached to avoid re-allocation each frame.
  private int indexOfCurrentTime;
  private float fractionalProgressThroughIndex;

  /// <summary>
  /// Sets scene lighting to match the specified time of day.
  /// <paramref name="hour"/> must be in 24-hour time between (0 - 23.999...).
  /// </summary>
  public void SetTime(float time)
  {
    if (time < 0 || time >= 24) throw new Exception("Time must be between 0 (inclusive) and 24 (exclusive), but was " + time + ".");

    // Light and background colour.
    indexOfCurrentTime = Mathf.FloorToInt(time / 6f);
    fractionalProgressThroughIndex = (time - (indexOfCurrentTime * 6f)) / 6f;

    sun.color = sunColor[indexOfCurrentTime].Evaluate(fractionalProgressThroughIndex);
    sun.intensity = sun.color.a;
    mainCamera.backgroundColor = skyColor[indexOfCurrentTime].Evaluate(fractionalProgressThroughIndex);

    // Directional light angle.
    indexOfCurrentTime = Mathf.FloorToInt(time);
    fractionalProgressThroughIndex = time - indexOfCurrentTime;
    if (indexOfCurrentTime + 1 < sunDirectionByHour.Length)
      sun.transform.rotation = Quaternion.Euler(Vector3.Lerp(
        a: sunDirectionByHour[indexOfCurrentTime],
        b: sunDirectionByHour[indexOfCurrentTime + 1],
        t: fractionalProgressThroughIndex));
    // For the very final moment (and end of the array), just use the last sun angle.
    else
      sun.transform.rotation = Quaternion.Euler(sunDirectionByHour[indexOfCurrentTime]);
  }
}
