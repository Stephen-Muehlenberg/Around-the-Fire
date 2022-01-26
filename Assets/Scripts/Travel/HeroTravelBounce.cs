using UnityEngine;

public class HeroTravelBounce : MonoBehaviour
{
  // TODO Make bounce height and speed based on hero condition (health, rest, hunger, mood)
  private enum BounceState { STOPPED, STOPPING, BOUNCING }
  private static readonly float BOUNCE_HEIGHT = 80;
  private static readonly float BOUNCE_SPEED = 3.5f;

  public Hero hero;
  private Vector3 origin;
  private BounceState state = BounceState.STOPPED;
  private float startTime;
  private float unscaledBounceY;
  private float previousUnscaledBounceY;
  private float yDelta;
  private float previousYDelta;

  private static float bounceMultiplier; // Cached to avoid frequent reallocations.

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
    if (state == BounceState.STOPPED)
    {
      startTime = Time.time;
      unscaledBounceY = 0;
      yDelta = 0;
    }
    state = BounceState.BOUNCING;
  }

  public void StopBouncing()
  {
    state = BounceState.STOPPING;
  }

  void Update()
  {
    if (state == BounceState.STOPPED) return;

    bounceMultiplier = 0.5f + (hero.health + hero.hunger + hero.rest + hero.mood) / 800f;

    previousUnscaledBounceY = unscaledBounceY;
    previousYDelta = yDelta;

    unscaledBounceY = Mathf.Abs(Mathf.Sin((Time.time - startTime) * bounceMultiplier * BOUNCE_SPEED));
    yDelta = unscaledBounceY - previousUnscaledBounceY;

    // Hit the ground and rebounded
    if (state == BounceState.STOPPING && previousYDelta <= 0 && yDelta > 0)
    {
      transform.localPosition = origin;
      state = BounceState.STOPPED;
    }

    // TODO Take into account character status, e.g. tired characters bounce less.
    transform.localPosition = origin + Vector3.up * BOUNCE_HEIGHT * bounceMultiplier * unscaledBounceY;
  }
}
