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

  /// <summary>
  /// Shows a stat changed popup above <paramref name="origin"/>, indicating <paramref name="stat"/>
  /// has changed by <paramref name="delta"/> amount.
  /// </summary>
  /// <param name="countOffset">Offsets the popup's position by one popup's worth of height. Use if need to show multiple popups at once without overlapping.</param>
  public static void Show(Portrait origin, Hero.Stat stat, int delta, int countOffset = 0)
  {
    if (origin == null) throw new System.Exception("Tried to show a StatPopup but the origin portrait was null.");
    if (delta == 0) {
      Debug.LogWarning("Tried to show a StatPopup with a value of 0. This request has been ignored.");
      return;
    }
    Show(origin: origin,
      text: DeltaToText(delta),
      color: delta > 0 ? Color.green : Color.red,
      spriteIndex: (int) stat,
      countOffset: countOffset,
      animateUp: true);
  }

  /// <summary>
  /// Shows <paramref name="text"/> message popup above <paramref name="origin"/>.
  /// </summary>
  /// <param name="countOffset">Offsets the popup's position by one popup's worth of height. Use if need to show multiple popups at once without overlapping.</param>
  /// <param name="animateUp">Determines the direction of the popup animation. True = up, false = down.</param>
  public static void Show(Portrait origin, string text, Color? color = null, int countOffset = 0, bool animateUp = true)
  {
    if (origin == null) throw new System.Exception("Tried to show a StatPopup but the origin portrait was null.");
    if (text == null || text.Length == 0)
    {
      Debug.LogWarning("Tried to show a StatPopup with null or empty text. This request has been ignored.");
      return;
    }
    Show(origin, text, color, spriteIndex: null, countOffset, animateUp);
  }

  private static void Show(
    Portrait origin, string text, Color? color,
    int? spriteIndex, int countOffset, bool animateUp)
  {
    // TODO Use object pooling.
    if (prefab == null)
      prefab = Resources.Load<GameObject>("Stat Popup");

    var instance = Instantiate(prefab, PopupController.canvasTransform);
    instance.transform.position = origin.transform.position + (Vector3.up * (140 + (countOffset * 45)));

    var statPopup = instance.GetComponent<StatPopup>();
    statPopup.image.enabled = spriteIndex != null;
    if (spriteIndex != null)
      statPopup.image.sprite = statPopup.statSprites[(int) spriteIndex];
    statPopup.text.text = text;
    if (color.HasValue)
      statPopup.text.color = color.Value;
    statPopup.directionMultiplier = animateUp ? 1 : -1;

    Destroy(instance, TIME_TO_LIVE);
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
