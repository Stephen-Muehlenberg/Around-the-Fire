using UnityEngine;

public class OptionHoverPopupPanel : MonoBehaviour
{
  [SerializeField] private TMPro.TMP_Text description;

  public void ShowFor(OptionButton button)
  {
    transform.position = button.transform.position;
    (transform as RectTransform).anchoredPosition = new Vector2(transform.localPosition.x, 0);
//    transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);
    description.text = button.option.hoverDescription;
  }
}
