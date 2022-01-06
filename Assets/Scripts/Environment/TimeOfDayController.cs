using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls scene lighting based on the specified time of day.
/// </summary>
public class TimeOfDayController : MonoBehaviour
{
  private static readonly int MIN_HOUR = 17; // 5pm
  private static readonly int MAX_HOUR = 24; // 12am
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

  private void PlayDemo(int _)
  {
    AdvanceTime(MIN_HOUR, MAX_HOUR - MIN_HOUR, null, PlayDemo);
  }

  /// <summary>
  /// Sets scene lighting to match the specified time of day.
  /// <paramref name="hour"/> must be in 24hour time between <see cref="MIN_HOUR"/> and <see cref="MAX_HOUR"/>.
  /// </summary>
  public static void SetTime(int hour)
  {
    if (hour < MIN_HOUR || hour > MAX_HOUR) throw new System.Exception("Hour must be between " + MIN_HOUR + " and " + MAX_HOUR + " inclusive.");
    singleton.SetTime(
      currentHourIndex: hour - MIN_HOUR,
      currentHourFraction: 0,
      totalTimeFraction: (hour - MIN_HOUR) / (MAX_HOUR - MIN_HOUR)
    );
  }

  /// <summary>
  /// Animate the scene lighting changing over time.
  /// <paramref name="startHour"/> is in 24hour time, and 
  /// the start and end times must be within <see cref="MIN_HOUR"/>
  /// and <see cref="MAX_HOUR"/>.
  /// </summary>
  public static void AdvanceTime(
    int startHour, int hours,
    Action<int> onHourEnd = null, Action<int> onFinished = null)
  {
    if (hours <= 0) throw new Exception("Time must be advanced by a positive number of hours but was " + hours + ".");
    if (startHour + hours > MAX_HOUR) throw new Exception("Can't advance time beyond " + MAX_HOUR + " hours (starting at " + startHour + ", duration " + hours + ").");
    singleton.StartCoroutine(singleton.AdvanceTimeCoroutine(startHour, hours, onHourEnd, onFinished));
  }

  /// <summary>
  /// Continuously advances time in a loop, to show off how it looks.
  /// </summary>
  public static void DemoAdvanceTime(int _)
  {
    AdvanceTime(MIN_HOUR, MAX_HOUR - MIN_HOUR, null, DemoAdvanceTime);
  }

  private IEnumerator AdvanceTimeCoroutine(
    int startHour, int hours, 
    Action<int> onHourEnd, Action<int> onFinish)
  {
    int endTime = startHour + hours;
    float totalDayDuration = MAX_HOUR - MIN_HOUR;

    float progress = 0; // Fraction of total progress, 0 = start, 1 = end.
    float currentTime = startHour; // World time in decimal, e.g. 2.5 = 2:30am.
    int currentHourIndex;
    int previousHourIndex = startHour - MIN_HOUR;

    // Advance time and update lighting in a loop.
    while (currentTime < endTime)
    {
      currentHourIndex = Mathf.FloorToInt(currentTime) - MIN_HOUR;

      // Invoke callback when we pass an hour mark.
      if (onHourEnd != null && previousHourIndex < currentHourIndex)
      {
        previousHourIndex++;
        onHourEnd.Invoke(currentHourIndex + MIN_HOUR);
      }

      // Update visuals.
      SetTime(
        currentHourIndex: currentHourIndex,
        currentHourFraction: currentTime - Mathf.Floor(currentTime),
        totalTimeFraction: (currentTime - MIN_HOUR) / totalDayDuration); ;

      // Advance time, then smooth step it for the purposes of animation.
      progress += Time.deltaTime / ADVANCE_SPEED_SECONDS_PER_HOUR / hours;
      currentTime = startHour + (Mathf.SmoothStep(0, hours, progress) * hours);

      yield return null;
    }

    // Snap to exactly the end time.
    SetTime(
      currentHourIndex: endTime - MIN_HOUR,
      currentHourFraction: 0,
      totalTimeFraction: (endTime - MIN_HOUR) / totalDayDuration);

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
