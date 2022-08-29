using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BounceMovement : MonoBehaviour
{
  private enum BounceState { STOPPED, STOPPING, BOUNCING }

  public bool bounceOnAwake = true;
  public bool addSlightVarianceToBounce = false;

  private Vector3 groundPoint;
  private BounceState state;
  private float bounceHeight = 50;
  private float bounceDuration = 1;
  private float yVelocity;

  private bool moving = false;
  private Vector3 moveDestination;
  private float? moveDuration;
  private bool bounceInPlaceAtDestination;
  private Action onReachDestination;

  private void Awake()
  {
    groundPoint = transform.localPosition;
    state = bounceOnAwake ? BounceState.BOUNCING : BounceState.STOPPED;
  }

  public void Initialise(float? bounceHeight = null, float? bounceDuration = null, bool? addSlightVarianceToBounce = null)
  {
    if (bounceHeight.HasValue)
      this.bounceHeight = bounceHeight.Value;
    if (bounceDuration.HasValue)
      this.bounceDuration = bounceDuration.Value;
    if (addSlightVarianceToBounce.HasValue)
      this.addSlightVarianceToBounce = addSlightVarianceToBounce.Value;
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

  public void SetBouncHeight(float height)
  {
    bounceHeight = height;
  }

  public void SetBounceDuration(float duration)
  {
    bounceDuration = duration;
  }

  public void SetGroundPointToCurrentPosition()
  {
    groundPoint = transform.localPosition;
  }

  void Update()
  {
    // TODO handle bounce height and duration modifiers
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

      // If bouncing, launch/bounce off ground;
      yVelocity = 400 + (addSlightVarianceToBounce ? UnityEngine.Random.Range(-20, 20) : 0);
    }

    transform.localPosition += Vector3.up * yVelocity * Time.deltaTime;
    if (transform.localPosition.y < groundPoint.y)
      transform.localPosition = groundPoint;

    yVelocity -= Time.deltaTime * 600f;
  }
}
