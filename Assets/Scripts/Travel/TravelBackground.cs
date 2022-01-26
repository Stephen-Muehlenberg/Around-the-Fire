using UnityEngine;

/// <summary>
/// Animates the travel scene's paralax background.
/// </summary>
public class TravelBackground : MonoBehaviour
{
  private const float MOUNTAIN_PAN_SPEED = 0.6f;
  private const float TREE_PAN_SPEED = 3;
  private const float OFFSCREEN_X_CUTOFF = -25;

  public Transform[] mountains;
  public Transform[] trees;

  /// <summary>
  /// Pans the paralax background. 1 <paramref name="distance"/> = 1 hour's travel
  /// under normal conditions.
  /// </summary>
  public void TravelDistance(float distance)
  {
    // TODO Psuedo-randomly generate backgrounds.

    foreach (Transform mountain in mountains)
    {
      mountain.localPosition += Vector3.left * distance * MOUNTAIN_PAN_SPEED;
      if (mountain.localPosition.x < OFFSCREEN_X_CUTOFF)
        mountain.localPosition += Vector3.left * OFFSCREEN_X_CUTOFF * 2;
    }

    foreach (Transform tree in trees)
    {
      tree.localPosition += Vector3.left * distance * TREE_PAN_SPEED;
      if (tree.localPosition.x < OFFSCREEN_X_CUTOFF)
        tree.localPosition += Vector3.left * OFFSCREEN_X_CUTOFF * 2;
    }
  }
}
