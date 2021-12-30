using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
  public CampAction action;
  [SerializeField] private Button button;
  [SerializeField] private Image tint;
  [SerializeField] private TMPro.TMP_Text title;
  [SerializeField] private TMPro.TMP_Text description;

  private static Color selectedColor = new Color(1, 0.8961949f, 0.504717f);
  private static Color unselectedColor = new Color(0, 0, 0, 0.6745098f);

  public void SetAction(CampAction action)
  {
    this.action = action;
    title.text = action.title;
    description.text = action.shortDesc;
  }

  public void SetSelected(bool selected)
  {
    tint.color = selected ? selectedColor : unselectedColor;
    button.enabled = !selected; ;
  }
}
