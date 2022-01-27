/// <summary>
/// Holds miscellanious utility methods.
/// </summary>
public static class Utils
{
  /// <summary>
  /// Formats time as am/pm, with the hour rounded down. E.g. 13.5 -> "1pm".
  /// </summary>
  public static string GetDisplayTime(float time)
  {
    int hour = (int) time;
    string ampm = hour < 12 ? "am" : "pm";
    if (hour > 12) hour -= 12;
    if (hour == 0) hour = 12;
    return hour.ToString() + ampm;
  }
}
