using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls scene lighting based on the specified time of day.
/// </summary>
public class TimeOfDayController : MonoBehaviour
{
  private static readonly float ADVANCE_SPEED_SECONDS_PER_HOUR = 2.5f;

  private static TimeOfDayController singleton;

  [SerializeField] private bool demoOnStart;
  [SerializeField] private Light sun;
  [SerializeField] private Camera mainCamera;
  [SerializeField] private Gradient sunColorByHour;
  [SerializeField] private Gradient skyColorByHour;
  [SerializeField] private Vector3[] sunDirectionByHour;

  private void Awake()
  {
    if (singleton != null) throw new System.Exception("Multiple singletons created.");
    singleton = this;
  }

  private void Start()
  {
    if (demoOnStart)
      PlayDemo(0);
  }

  /// <summary>
  /// Continuously advances time in a loop, to show off how it looks.
  /// </summary>
  private void PlayDemo(float _)
  {
    AdvanceTime(0, 24, null, PlayDemo);
  }

  /// <summary>
  /// Sets scene lighting to match the specified time of day.
  /// <paramref name="hour"/> must be in 24hour time between <see cref="MIN_HOUR"/> and <see cref="MAX_HOUR"/>.
  /// </summary>
  public static void SetTime(float time)
  {
    int hour = Mathf.FloorToInt(time);
    singleton.SetTime(
      currentHourIndex: hour,
      currentHourFraction: time - (int) time,
      totalTimeFraction: hour / 24
    );
  }

  /// <summary>
  /// Animate the scene lighting changing over time.
  /// <paramref name="startHour"/> is in 24hour time, and 
  /// the start and end times must be within <see cref="MIN_HOUR"/>
  /// and <see cref="MAX_HOUR"/>.
  /// </summary>
  public static void AdvanceTime(
    float startTime, int hours,
    Action<int> onHourEnd = null, Action<float> onFinished = null)
  {
    if (hours <= 0) throw new Exception("Time must be between 1 and 24 but was " + hours + ".");
    singleton.StartCoroutine(singleton.AdvanceTimeCoroutine(startTime, hours, onHourEnd, onFinished));
  }

  private IEnumerator AdvanceTimeCoroutine(
    float startTime, int hours, 
    Action<int> onHourEnd, Action<float> onFinish)
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
      SetTime(
        currentHourIndex: currentHourIndex,
        currentHourFraction: currentTime - Mathf.Floor(currentTime),
        totalTimeFraction: currentTime / 24);

      // Advance time, then smooth step it for the purposes of animation.
      progress += Time.deltaTime / ADVANCE_SPEED_SECONDS_PER_HOUR / hours;
      currentTime = startTime + (Mathf.SmoothStep(0, hours, progress) * hours);

      yield return null;
    }

    // Snap to exactly the end time.
    SetTime(
      currentHourIndex: (int) endTime,
      currentHourFraction: 0,
      totalTimeFraction: endTime / 24);

    // Invoke final callback.
    if (onFinish != null) onFinish.Invoke(endTime);
  }

  private void SetTime(int currentHourIndex, float currentHourFraction, float totalTimeFraction)
  {
    sun.color = sunColorByHour.Evaluate(totalTimeFraction);
    sun.intensity = sun.color.a;

    // For all times except the final moment, Lerp between different sun angles.
    if (currentHourIndex + 1 < sunDirectionByHour.Length)
      sun.transform.rotation = Quaternion.Euler(Vector3.Lerp(
        a: sunDirectionByHour[currentHourIndex],
        b: sunDirectionByHour[currentHourIndex + 1],
        t: currentHourFraction));
    // For the very final moment (and end of the array), just use the last sun angle.
    else
      sun.transform.rotation = Quaternion.Euler(sunDirectionByHour[currentHourIndex]);

    mainCamera.backgroundColor = skyColorByHour.Evaluate(totalTimeFraction);
  }
}
