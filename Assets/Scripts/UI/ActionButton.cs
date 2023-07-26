using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
  [SerializeField] private Button button;
  [SerializeField] private Image tint;
  [SerializeField] private TMPro.TMP_Text title;
  [SerializeField] private TMPro.TMP_Text hoverText;
  [SerializeField] private GameObject unavailableUI;
  [SerializeField] private TMPro.TMP_Text unavailableReason;

  public class Content
  {
    public string text;
    public string hoverText;
    public int state;
  }

  public void Set(Content content)
  {
    Debug.Log(name + ".ActionButton.Set() content ? " + (content != null));
    gameObject.SetActive(content != null);
    if (content == null) return;

    title.text = content.text;
  //  hoverText.text = content.hoverText;
  //  unavailableUI.SetActive(content.state > 0);
 //   unavailableReason.text = content.state switch
 //   {
//      HeroAction.Availability.NEEDS_A_FIRE => "Needs a Fire",
 //     HeroAction.Availability.NOT_ENOUGH_SUPPLIES => "Not enough Supplies",
  //    HeroAction.Availability.NOT_ENOUGH_WOOD => "Not enough Wood",
  //    _ => ""
  //  };
  }
}
