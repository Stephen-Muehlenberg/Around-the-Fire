using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

  public void Initialise(Character character, Interactions interactions = Interactions.NONE)
  {
    this.character = character;
    nameText.text = character.name;
    portrait.sprite = character.icon;
    this.interactions = interactions;
    SetHighlighted(false);
    SetSelected(false);
  }

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
    public void OnPointerEnter(Portrait portrait);
    public void OnPointerExit(Portrait portrait);
    public void OnPointerClick(Portrait portrait);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (interactions != Interactions.CLICKABLE) return;
    if (selectOnClick) SetSelected(true);
    callbacks?.OnPointerClick(this);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    SetHighlighted(true);
    callbacks?.OnPointerEnter(this);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    SetHighlighted(false);
    callbacks?.OnPointerExit(this);
  }
}
