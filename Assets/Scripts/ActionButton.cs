using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
  public CampAction action;
  [SerializeField] private Button button;
  [SerializeField] private Image tint;
  [SerializeField] private TMPro.TMP_Text title;
  [SerializeField] private TMPro.TMP_Text description;

  public void SetAction(CampAction action)
  {
    this.action = action;
    title.text = action.title;
    description.text = action.description;
  }
}
