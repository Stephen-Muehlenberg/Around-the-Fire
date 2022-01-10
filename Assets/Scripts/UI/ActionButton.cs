using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
  public HeroAction action;
  [SerializeField] private Button button;
  [SerializeField] private Image tint;
  [SerializeField] private TMPro.TMP_Text title;
  [SerializeField] private TMPro.TMP_Text description;
  [SerializeField] private GameObject unavailableUI;
  [SerializeField] private TMPro.TMP_Text unavailableReason;

  public void SetAction(HeroAction action, HeroAction.Availability availability)
  {
    this.action = action;
    title.text = action.title;
    description.text = action.description;
    unavailableUI.SetActive(availability != HeroAction.Availability.AVAILABLE);
    unavailableReason.text = availability switch
    {
      HeroAction.Availability.NEEDS_A_FIRE => "Needs a Fire",
      HeroAction.Availability.NOT_ENOUGH_SUPPLIES => "Not enough Supplies",
      HeroAction.Availability.NOT_ENOUGH_WOOD => "Not enough Wood",
      _ => ""
    };
  }
}
