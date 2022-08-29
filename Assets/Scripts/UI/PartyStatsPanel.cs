using UnityEngine;

/// <summary>
/// Displays the time and party stats.
/// </summary>
public class PartyStatsPanel : MonoBehaviour
{
  [SerializeField] private TMPro.TMP_Text textField;

  public void Start()
  {
    Display(Party.currentState);
    Party.currentState.updates += Display;
  }

  public void OnDestroy()
    => Party.currentState.updates -= Display;

  public void Display(PartyState state)
  {
    textField.text = "Time: " + Utils.GetDisplayTime(state.timeOfDay)
      + "\nSupplies: " + Mathf.FloorToInt(state.supplies) + " (" + Mathf.FloorToInt(state.supplies / 4f / state.heroes.Count) + " days)"
      + "\nWood: " + Mathf.FloorToInt(state.firewood) + " (" + Mathf.FloorToInt(state.firewood / 8) + " days)";
  }
}
