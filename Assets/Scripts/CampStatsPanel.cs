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

  public static void SetStats(int time)
  {
    singleton.time.text = (time - 12).ToString() + "pm";
  }
}
