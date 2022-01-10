using UnityEngine;

public class CampStatsPanel : MonoBehaviour
{
  private static CampStatsPanel singleton;

  [SerializeField] private TMPro.TMP_Text time;

  private void Awake()
  {
    if (singleton != null) throw new System.Exception("Cannot have two CampStatsPanel singletons.");
    singleton = this;
  }

  public static void Display(CampState state)
  {
    singleton.time.text = "Time: " + (state.hour - 12).ToString() + "pm"
      + "\nSupplies: " + Mathf.FloorToInt(state.supplies) + " (" + Mathf.FloorToInt(state.supplies / 4f / state.heroes.Count) + " days)"
      + "\nWood: " + Mathf.FloorToInt(state.firewood) + " (" + Mathf.FloorToInt(state.firewood / 8) + " days)";
  }
}
