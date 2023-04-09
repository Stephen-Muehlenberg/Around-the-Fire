using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// An area of the screen for displaying all the <see cref="Portrait"/>s
/// in a given <see cref="PortraitZone"/>. Consists of 1
/// or more <see cref="PortraitZoneUiArea"/>s.
/// </summary>
public class PortraitZoneUiGroup : MonoBehaviour
{
  public interface Interactions
  {
    public void OnPointerEnterZone(PortraitZoneUiGroup zoneUi, PortraitZoneUiArea childEntered);
    public void OnPointerExitZone(PortraitZoneUiGroup zoneUi, PortraitZoneUiArea childExited);
    public void OnClickZone(PortraitZoneUiGroup zoneUi, PortraitZoneUiArea childClicked, PointerEventData data);
  }

  public enum Appearance { HIDDEN, VALID_TARGET, INVALID_TARGET }

  public PortraitZone zone { get; private set; }
  [field: SerializeField] public List<PortraitZoneUiArea> childAreas { get; private set; }
  public bool distributePortraitsEvenly = true;
  [SerializeField] private Color validHighlight;
  [SerializeField] private Color invalidHighlight;
  public List<Portrait> portraits { get; private set; }
  public Interactions callback;

  public void Initialise(PortraitZone zone, Interactions callback)
  {
    this.zone = zone;
    portraits = new();
    this.callback = callback;
    SetAppearance(Appearance.HIDDEN);
  }

  public void SetAppearance(Appearance appearance)
  {
    Color color = appearance switch
    {
      Appearance.VALID_TARGET => validHighlight,
      Appearance.INVALID_TARGET => invalidHighlight,
      _ => Color.clear
    };
    childAreas.ForEach(it => it.SetHighlightColor(color));
  }

  public bool CanAccept(Portrait portrait)
    => childAreas.Any(it => it.CanAccept(portrait));

  public void Add(Portrait portrait, PortraitZoneUiArea specificArea = null)
  {
    if (!CanAccept(portrait))
      throw new System.Exception("Can't add portrait; not enough space!");

    portraits.Add(portrait);
    if (specificArea != null && specificArea.CanAccept(portrait))
      specificArea.Add(portrait);
    else
      childAreas
        .OrderBy(it => it.portraits.Count)
        .First(it => it.CanAccept(portrait))
        .Add(portrait);
  }

  public void Remove(Portrait portrait)
  {
    portraits.Remove(portrait);
    childAreas.ForEach(it => it.portraits.Remove(portrait));
  }

  public void OnPointerEnter(PortraitZoneUiArea childArea)
  {
    if (callback != null)
      callback.OnPointerEnterZone(this, childArea);
  }

  public void OnPointerExit(PortraitZoneUiArea childArea)
  {
    if (callback != null)
      callback.OnPointerExitZone(this, childArea);
  }

  public void OnPointerClick(PortraitZoneUiArea childArea, PointerEventData data)
  {
    if (callback != null)
      callback.OnClickZone(this, childArea, data);
  }
}
