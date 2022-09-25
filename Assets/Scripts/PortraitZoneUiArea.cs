using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// An area of the screen corresponding to a <see cref="PortraitZone"/>,
/// as part of a <see cref="PortraitZoneUiGroup"/>.
/// </summary>
public class PortraitZoneUiArea : MonoBehaviour,
  IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
  public PortraitZoneUiGroup parentGroup;
  [SerializeField] private Image highlight;
  [SerializeField] private Transform portraitTransformParent;
  public List<Portrait> portraits;
  public int maxPortraits;

  public void SetHighlightColor(Color color)
  {
    highlight.color = color;
  }

  public bool CanAccept(Portrait portrait)
  {
    return portraits.Count < maxPortraits
      && !portraits.Contains(portrait);
  }

  public void Add(Portrait portrait)
  {
    if (!CanAccept(portrait))
      throw new System.Exception("Can't accept portrait " + portrait.character.name + " at " + parentGroup.zone.name); ;

    portrait.transform.SetParent(portraitTransformParent);
    portraits.Add(portrait);
  }

  public void OnPointerEnter(PointerEventData eventData)
    => parentGroup.OnPointerEnter(this);

  public void OnPointerExit(PointerEventData eventData)
    => parentGroup.OnPointerExit(this);

  public void OnPointerClick(PointerEventData eventData)
    => parentGroup.OnPointerClick(this, eventData);
}
