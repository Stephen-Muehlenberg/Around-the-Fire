using System.Collections;
using UnityEngine;

public class TimePopup : MonoBehaviour
{
  [SerializeField] private CanvasGroup canvasGroup;
  [SerializeField] private TMPro.TMP_Text text;

  public static IEnumerator Show(float time)
  {
    var prefab = Resources.Load<GameObject>("Time Popup");
    var instance = Instantiate(prefab, PopupController.canvasTransform);

    // Set display text.
    var popup = instance.GetComponent<TimePopup>();
    popup.text.text = Utils.GetDisplayTime(time);

    // Animate popup from right to left, slowing at the center.
    var transform = popup.transform;
    transform.localPosition = Vector3.right * 135;
    float t = 0;
    float v = 400;
    float dv = 750;

    // Start fast, decelerate in.
    while (t < 0.5f)
    {
      yield return null;
      t += Time.deltaTime;
      transform.localPosition += Vector3.left * v * Time.deltaTime;
      v -= dv * Time.deltaTime;
      popup.canvasGroup.alpha = t * 2;
    }

    // Crawl at a constant rate.
    popup.canvasGroup.alpha = 1;
    while (t < 1.5f)
    {
      yield return null;
      t += Time.deltaTime;
      transform.localPosition += Vector3.left * v * Time.deltaTime;
    }

    // Accelerate out.
    while (t < 2f)
    {
      yield return null;
      t += Time.deltaTime;
      v += dv * Time.deltaTime;
      transform.localPosition += Vector3.left * v * Time.deltaTime;
      popup.canvasGroup.alpha = 4 - (t * 2);
    }

    Destroy(instance);
  }
}
