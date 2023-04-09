using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Provides inspector hooks for hovering over a UI element.
/// </summary>
public class OnHover : MonoBehaviour,
  IPointerEnterHandler, IPointerExitHandler
{
  public UnityEvent onHoverStart;
  public UnityEvent onHoverStop;

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (onHoverStart != null)
      onHoverStart.Invoke();
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (onHoverStart != null)
      onHoverStop.Invoke();
  }
}
