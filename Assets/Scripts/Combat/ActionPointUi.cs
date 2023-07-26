using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPointUi : MonoBehaviour
{
  [SerializeField] private CanvasGroup[] pointIcons;
  [SerializeField] private Slider slider;
  [SerializeField] private TMPro.TMP_Text bonusText;

  /// <summary>
  /// Shows the specified amount of AP, ranging from 0 to 3.
  /// </summary>
  public void ShowAP(float actionPoints)
  {
    for (int i = 0; i < 3; i++)
      pointIcons[i].alpha = actionPoints >= i + 1
        ? 1f
        : 0.2f;

    slider.value = actionPoints;
    bonusText.text = actionPoints == 3
      ? "<color=#FFE600>Bonus: +15"
      : actionPoints >= 2
      ? "<color=#F1FF39>Bonus: +10"
      : actionPoints >= 1
      ? "<color=#B8FFFF>Bonus: +5"
      : "<color=#ECECEC88>Bonus: 0";
  }
}
