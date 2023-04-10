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
    textField.text = "Time: " + Utils.GetDisplayTime(state.world.time.hourOfDay)
      + "\nFood: " + state.party.inventory.food + " (" + state.party.inventory.daysWorthOfSupplies(state.party) + " days)"
      + "\nFirewood: " + state.party.inventory.firewood + " (" + state.party.inventory.daysWorthOfFirewood(state.party) + " days)";
  }
}
