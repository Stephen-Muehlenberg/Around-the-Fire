using System;
using System.Collections;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
  private static float REVEAL_DELAY_PER_CHAR = 0.03f;
  private static float REVEAL_END_DELAY = 1.4f;
  private static GameObject prefab;

  [SerializeField] private TMPro.TMP_Text text;

  // TODO use object pooling
  public static void Show(AdventurerPortrait adventurer, string text, Action callback = null)
  {
    if (prefab == null)
      prefab = Resources.Load<GameObject>("Speech Bubble");

    var instance = Instantiate(prefab, PopupController.canvasTransform);
    instance.GetComponent<SpeechBubble>().Initialise(text, adventurer.transform, callback);
  }

  private void Initialise(string text, Transform origin, Action callback)
  {
    transform.position = origin.position + (Vector3.up * 90);
    StartCoroutine(Reveal(text, callback));
  }

  private IEnumerator Reveal(string text, Action callback)
  {
    string revealed = "";
    int nextChar = 0;

    while (revealed.Length < text.Length)
    {
      revealed += text[nextChar];
      this.text.text = revealed;
      nextChar++;
      yield return new WaitForSeconds(REVEAL_DELAY_PER_CHAR);
    }

    yield return new WaitForSeconds(REVEAL_END_DELAY);
    Destroy(this.gameObject);
    if (callback != null)
      callback.Invoke();
  }
}
