using UnityEngine;

/// <summary>
/// Animates the travel scene's parallax background.
/// </summary>
public class TravelBackground : MonoBehaviour
{
  private const float OFFSCREEN_X_CUTOFF = -25;

  private Transform[] parallaxElements;

  private void Awake()
  {
    parallaxElements = new Transform[transform.childCount];
    for (int i = 0; i < transform.childCount; i++)
      parallaxElements[i] = transform.GetChild(i);
  }

  /// <summary>
  /// Pans the parallax background by the specified <paramref name="distance"/>.
  /// </summary>
  public void TravelDistance(float distance)
  {
    // TODO Psuedo-randomly generate backgrounds.

    foreach (Transform element in parallaxElements)
    {
      element.localPosition += Vector3.left * distance / element.localPosition.z;
      if (element.localPosition.x < OFFSCREEN_X_CUTOFF)
        element.localPosition += Vector3.left * OFFSCREEN_X_CUTOFF * 2;
    }
  }
}
