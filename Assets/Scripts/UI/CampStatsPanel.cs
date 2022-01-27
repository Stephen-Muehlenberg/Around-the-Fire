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

  public static void Display(PartyState state)
  {
    singleton.time.text = "Time: " + Utils.GetDisplayTime(state.time)
      + "\nSupplies: " + Mathf.FloorToInt(state.supplies) + " (" + Mathf.FloorToInt(state.supplies / 4f / state.heroes.Count) + " days)"
      + "\nWood: " + Mathf.FloorToInt(state.firewood) + " (" + Mathf.FloorToInt(state.firewood / 8) + " days)";
  }
}
