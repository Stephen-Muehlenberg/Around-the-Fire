using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelInventoryUi : MonoBehaviour
{
  [SerializeField] private TMPro.TMP_Text supppliesFresh;
  [SerializeField] private TMPro.TMP_Text supppliesRation;
  [SerializeField] private TMPro.TMP_Text firewood;
  [SerializeField] private TMPro.TMP_Text money;

  public void UpdateInventoryUi(Inventory inventory)
  {
    supppliesFresh.text = inventory.foodFresh.ToString();
    supppliesRation.text = inventory.foodCured.ToString();
    firewood.text = inventory.firewood.ToString("n0");
    money.text = inventory.money.ToString();
  }
}
