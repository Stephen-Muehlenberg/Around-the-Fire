using UnityEngine;

/// <summary>
/// Displays the time and party stats.
/// </summary>
public class PartyStatsPanel : MonoBehaviour
{
  [SerializeField] private TMPro.TMP_Text textField;

  public void Start()
  {
    Display(Game.state);
    Game.onCampaignStateChanged += Display;
  }

  public void OnDestroy()
    => Game.onCampaignStateChanged -= Display;

  public void Display(GameState state)
  {
    float supplies = state.party.inventory.supplies;
    float firewood = state.party.inventory.firewood;
    textField.text = "Time: " + Utils.GetDisplayTime(state.world.time.hourOfDay)
      + "\nSupplies: " + Mathf.FloorToInt(supplies) + " (" + state.party.inventory.daysWorthOfSupplies(state.party) + " days)"
      + "\nWood: " + Mathf.FloorToInt(firewood) + " (" + state.party.inventory.daysWorthOfFirewood(state.party) + " days)";
  }
}
