using UnityEngine;

/// <summary>
/// Animates hero portrait bouncing up and down.
/// </summary>
public class HeroTravelBounce : MonoBehaviour
{
  // TODO Make bounce height and speed based on hero condition (health, rest, hunger, mood)
  private enum BounceState { STOPPED, STOPPING, BOUNCING }

  public Hero hero;
  private Vector3 origin;
  private BounceState state = BounceState.STOPPED;
  private float yVelocity;

  public void Initialize(Hero hero)
  {
    this.hero = hero;

    float randomStartDelay = Random.Range(0.1f, 0.3f);
    this.DoAfterDelay(randomStartDelay, () => {
      origin = transform.localPosition;
      StartBouncing();
    });
  }

  public void StartBouncing()
  {
    state = BounceState.BOUNCING;
  }

  public void StopBouncing()
  {
    state = BounceState.STOPPING;
  }

  void Update()
  {
    if (state == BounceState.STOPPED) return;

    // If on ground, start bouncing.
    if (transform.localPosition.y <= origin.y)
    {
      if (state == BounceState.STOPPING)
      {
        state = BounceState.STOPPED;
        return;
      }

      float bounceMultiplier = 0.4f + (hero.health + hero.hunger + hero.rest + hero.mood) / 666f;
      yVelocity = 400 * bounceMultiplier;
    }

    transform.localPosition += Vector3.up * yVelocity * Time.deltaTime;
    if (transform.localPosition.y < origin.y)
      transform.localPosition = origin;

    yVelocity -= Time.deltaTime * 600f;
  }
}
