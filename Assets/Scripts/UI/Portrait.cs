using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A standard representation of a <see cref="Character"/>.
/// </summary>
public class Portrait : MonoBehaviour,
  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
  public enum Interactions { NONE, HIGHLIGHTABLE, CLICKABLE }

  [SerializeField] private Image portrait;
  [SerializeField] private Image highlight;
  [SerializeField] private Image selection;
  [SerializeField] private TMPro.TMP_Text nameText;
  [SerializeField] private TMPro.TMP_Text actionText;

  public Character character { get; private set; }
  public Interactions interactions;
  public EventsCallback callbacks;
  public bool selectOnClick;
  public bool highlighted { get; private set; }
  public bool selected { get; private set; }
  public Sprite sprite => portrait.sprite;

  public void Initialise(Character character, Interactions interactions = Interactions.NONE, EventsCallback callbacks = null)
  {
    this.character = character;
    nameText.text = character.name;
    portrait.sprite = character.icon;
    this.interactions = interactions;
    this.callbacks = callbacks;
    SetHighlighted(false);
    SetSelected(false);
  }

  public void ShowName(bool show) => nameText.gameObject.SetActive(show);

  public void SetAction(string action)
  {
    bool actionSelected = action != null && action.Length > 0;
    actionText.gameObject.SetActive(actionSelected);
    if (actionSelected)
      actionText.text = action;
  }

  public void SetHighlighted(bool highlighted)
  {
    this.highlighted = highlighted;
    highlight.gameObject.SetActive(highlighted);
    actionText.color = highlighted ? Color.yellow : Color.white;
  }

  public void SetSelected(bool selected)
  {
    this.selected = selected;
    selection.gameObject.SetActive(selected);
  }

  public void SetInteractions(Interactions interactions)
  {
    this.interactions = interactions;

    // If this isn't interactable, let raycasts pass through it.
    bool raycastTargetEnabled = interactions != Interactions.NONE;
    portrait.raycastTarget = raycastTargetEnabled;
    highlight.raycastTarget = raycastTargetEnabled;
    selection.raycastTarget = raycastTargetEnabled;
    nameText.raycastTarget = raycastTargetEnabled;
  }

  public interface EventsCallback
  {
    public void OnPointerEnterPortrait(Portrait portrait) { }
    public void OnPointerExitPortrait(Portrait portrait) { }
    public void OnPortraitLeftClick(Portrait portrait) { }
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    // If not a left click, try to invoke OnPointerClick on the first Zone below this.
    if (eventData.button != PointerEventData.InputButton.Left)
    {
      List<RaycastResult> results = new();      
      GetComponentInParent<GraphicRaycaster>().Raycast(eventData, results);
      PortraitZoneUiArea zone;
      for (int i = 0; i < results.Count; i++)
      {
        zone = results[i].gameObject.GetComponent<PortraitZoneUiArea>();
        if (zone != null)
        {
          zone.OnPointerClick(eventData);
          break;
        }
      }

      return;
    }

    if (interactions != Interactions.CLICKABLE) return;
    if (selectOnClick) SetSelected(true);
    callbacks?.OnPortraitLeftClick(this);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (interactions == Interactions.NONE) return;
    SetHighlighted(true);
    callbacks?.OnPointerEnterPortrait(this);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (interactions == Interactions.NONE) return;
    SetHighlighted(false);
    callbacks?.OnPointerExitPortrait(this);
  }
}
