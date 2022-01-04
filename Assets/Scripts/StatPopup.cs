using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates and animates a little popup displaying a stat increase/decrease.
/// </summary>
public class StatPopup : MonoBehaviour
{
  private static readonly float TIME_TO_LIVE = 1.5f;
  private static readonly float ANIMATE_SPEED = 10f;
  private static GameObject prefab;

  [SerializeField] private Image image;
  [SerializeField] private TMPro.TMP_Text text;
  // TODO This is a lazy resource loading hack. Consider cleaning it up sometime.
  [SerializeField] private Sprite[] statSprites;

  private int directionMultiplier;

  public static void Show(HeroPortrait origin, Hero.Stat stat, int delta)
  {
    if (delta == 0) return;

    // TODO Use object pooling.
    if (prefab == null)
      prefab = Resources.Load<GameObject>("Stat Popup");

    var instance = Instantiate(prefab, PopupController.canvasTransform);
    instance.transform.position = origin.transform.position + (Vector3.up * 140);

    var statPopup = instance.GetComponent<StatPopup>();
    statPopup.image.sprite = statPopup.statSprites[(int) stat];
    statPopup.text.text = DeltaToText(delta);
    statPopup.text.color = delta > 0 ? Color.green : Color.red;
    statPopup.directionMultiplier = delta > 0 ? 1 : -1;

    Destroy(instance.gameObject, TIME_TO_LIVE);
  }

  private static string DeltaToText(int delta)
  {
    if (delta > 0)
    {
      if (delta <= 10) return "+";
      if (delta <= 30) return "++";
      return "+++";
    }
    if (delta >= -10) return "-";
    if (delta >= -30) return "--";
    return "---";
  }

  private void Update()
  {
    transform.position += Vector3.up * Time.deltaTime * ANIMATE_SPEED * directionMultiplier;
  }
}
