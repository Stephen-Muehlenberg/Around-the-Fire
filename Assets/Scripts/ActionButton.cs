using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
  public HeroAction action;
  [SerializeField] private Button button;
  [SerializeField] private Image tint;
  [SerializeField] private TMPro.TMP_Text title;
  [SerializeField] private TMPro.TMP_Text description;

  public void SetAction(HeroAction action)
  {
    this.action = action;
    title.text = action.title;
    description.text = action.description;
  }
}
