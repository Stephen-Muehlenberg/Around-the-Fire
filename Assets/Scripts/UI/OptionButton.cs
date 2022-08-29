using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI Button for selecting a single <see cref="Option"/> from an
/// <see cref="SelectOptionUI"/>.
/// </summary>
public class OptionButton : MonoBehaviour,
  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField] private TMPro.TMP_Text title;
  [SerializeField] private Image icon;
  [SerializeField] private TMPro.TMP_Text overlayText;
  [SerializeField] private TMPro.TMP_Text bottomText;
  [SerializeField] private Image highlight;
  [SerializeField] private Image greyedOutTint;

  private SelectOptionUI group;
  public Option option { get; private set; }
  public int index { get; private set; }

  public void Show(Option option, int index, SelectOptionUI group)
  {
    this.group = group;
    this.option = option;
    this.index = index;
    title.text = option.title;
    icon.sprite = option.icon;
    overlayText.text = option.textOverlay;
    bottomText.text = option.bottomText;
    greyedOutTint.gameObject.SetActive(option.unavailable);
  }

  public void SetHighlighted(bool highlighted)
  {
    highlight.gameObject.SetActive(highlighted);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    group.OnButtonClick(this);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    group.OnButtonHoverStart(this);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    group.OnButtonHoverStop(this);
  }
}
