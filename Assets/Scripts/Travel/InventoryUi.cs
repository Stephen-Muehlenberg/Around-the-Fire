using UnityEngine;

public class InventoryUi : MonoBehaviour
{
  [SerializeField] private TMPro.TMP_Text supppliesFresh;
  [SerializeField] private TMPro.TMP_Text supppliesRation;
  [SerializeField] private TMPro.TMP_Text firewood;
  [SerializeField] private TMPro.TMP_Text money;

  private void OnEnable()
  {
    Game.party.inventory.onInventoryChanged += UpdateInventoryUi;
    UpdateInventoryUi(Game.party.inventory);
  }

  private void OnDisable()
  {
    if (Game.party?.inventory != null)
      Game.party.inventory.onInventoryChanged -= UpdateInventoryUi;
  }

  public void UpdateInventoryUi(Inventory inventory)
  {
    supppliesFresh.text = inventory.foodFresh.ToString();
    supppliesRation.text = inventory.foodCured.ToString();
    firewood.text = inventory.firewood.ToString("n0");
    money.text = inventory.money.ToString();
  }
}
