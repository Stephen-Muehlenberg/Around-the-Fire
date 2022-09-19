using System;
using UnityEngine;

public class BounceMovement : MonoBehaviour
{
  private enum BounceState { STOPPED, STOPPING, BOUNCING }

  /// <summary>Bounce height is randomly up to 5% higher or lower.</summary>
  public bool addSlightVarianceToBounce = false;

  private Vector3 groundPoint;
  private BounceState state;
  private float bounceHeight = 400; // Just some ok default values, nothing special.
  private float bounceDuration = 1.1547f; // Just some ok default values, nothing special.
  private float bounceGravity;
  private float bounceInitialVelocity;
  private float yVelocity;

  private bool moving = false;
  private Vector3 moveDestination;
  private float? moveDuration;
  private bool bounceInPlaceAtDestination;
  private Action onReachDestination;

  private void Awake()
  {
    groundPoint = transform.localPosition;
    state = BounceState.STOPPED;
  }

  public void Initialise(float? bounceHeight = null, float? bounceDuration = null, bool? addSlightVarianceToBounce = null)
  {
    if (bounceHeight.HasValue)
      this.bounceHeight = bounceHeight.Value;
    if (bounceDuration.HasValue)
      this.bounceDuration = bounceDuration.Value;
    if (addSlightVarianceToBounce.HasValue)
      this.addSlightVarianceToBounce = addSlightVarianceToBounce.Value;

    RecalculateBounceVariables();
  }

  public void SetBouncHeight(float height)
  {
    bounceHeight = height;
    RecalculateBounceVariables();
  }

  public void SetBounceDuration(float duration)
  {
    bounceDuration = duration;
    RecalculateBounceVariables();
  }

  private void RecalculateBounceVariables()
  {
    // Google "projectile motion equations" for an explanation of the maths.
    bounceGravity = 2 * bounceHeight / Mathf.Pow(bounceDuration, 2);
    bounceInitialVelocity = bounceGravity * bounceDuration / 2;
  }

  public void MoveTo(Vector3 destination, float? duration = null, Action callback = null)
  {
    moveDestination = destination;
    moveDuration = duration;
    bounceInPlaceAtDestination = false;
    onReachDestination = callback;
  }

  public void BounceTo(Vector3 destination, float? duration = null, bool stopBouncingAtDestination = false, Action callback = null)
  {
    moveDestination = destination;
    moveDuration = duration;
    bounceInPlaceAtDestination = !stopBouncingAtDestination;
    onReachDestination = callback;
  }

  public void StartBouncing()
  {
    state = BounceState.BOUNCING;
  }

  public void StopBouncing()
  {
    if (state == BounceState.BOUNCING)
      state = BounceState.STOPPING;
  }

  public void SetGroundPointToCurrentPosition()
  {
    groundPoint = transform.localPosition;
  }

  void Update()
  {
    // TODO handle movement.

    if (state == BounceState.STOPPED) return;

    // Check if we've reached the ground.
    if (transform.localPosition.y <= groundPoint.y)
    {
      // If stopping, change state to stopped.
      if (state == BounceState.STOPPING)
      {
        state = BounceState.STOPPED;
        transform.localPosition = groundPoint;
        return;
      }

      // If bouncing, launch/bounce off ground.
      yVelocity = bounceInitialVelocity * (addSlightVarianceToBounce ? UnityEngine.Random.Range(0.95f, 1.05f) : 1);
    }

    transform.localPosition += Vector3.up * yVelocity * Time.deltaTime;
    if (transform.localPosition.y < groundPoint.y)
      transform.localPosition = groundPoint;

    yVelocity -= Time.deltaTime * bounceGravity;
  }
}
