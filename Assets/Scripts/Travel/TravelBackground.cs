using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Animates the travel scene's parallax background.
/// </summary>
public class TravelBackground : MonoBehaviour
{
  private const float OFFSCREEN_X_CUTOFF = -25;

  [SerializeField] private GameObject mountainPrefab;
  [SerializeField] private GameObject hillPrefab;
  [SerializeField] private GameObject forestPrefab;
  [SerializeField] private GameObject cloudPrefab;

  private Transform[] parallaxElements;
  private ObjectPool<GameObject> clouds;

  public void Initialise()
  {
    // TODO pass in some parameters here, e.g. are we on a road or in a forest or on a mountain
    // TODO spawn initial parallax objects.
    // TODO figure out how to change environment over time, e.g. increase wood density
    //    probably need to spawn upcoming environments a set distance ahead.
    //    a given amount of screen space should correspond to journey kilometers.
  }

  public void UpdateParallax(float distanceTravelled, float windSpeed)
  {
    // TODO move objects based on distance travelled / distance from camera. 
    // Clouds also add the wind speed as a constant, even if distance travelled is zero.
    // If any objects go entirely off screen, return them to the pool.
    // Spawn in new objects based on some directions provided in Initialise().
  }

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
