using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroPortrait : MonoBehaviour,
  IBeginDragHandler, IDragHandler, IEndDragHandler,
  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
  public Hero hero { get; private set; }
  public CampLocation location;

  private Canvas canvas;
  private GraphicRaycaster raycaster;
  [SerializeField] private Image portrait;
  [SerializeField] private Image highlight;
  [SerializeField] private TMPro.TMP_Text nameText;
  [SerializeField] private TMPro.TMP_Text actionText;
  [SerializeField] private Button cancelButton;

  public static HeroPortrait selected;
  public static bool dragInProgress; // Don't highlight heroes while dragging.

  public void Initialise(Hero hero, CampLocation location)
  {
    this.hero = hero;
    nameText.text = hero.name;
    portrait.sprite = hero.icon;
    this.location = location;
    location.Add(this);
    canvas = GetComponentInParent<Canvas>();
    raycaster = GetComponentInParent<GraphicRaycaster>();
    SelectAction(null);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (dragInProgress) return;
    Select();
  }

  public void OnPointerExit(PointerEventData eventData)
  {
  }

  public void OnPointerClick(PointerEventData eventData)
  {
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    dragInProgress = true;
    portrait.raycastTarget = false;
    highlight.raycastTarget = false;
    nameText.raycastTarget = false;

    Select();

    transform.localPosition += new Vector3(
      eventData.delta.x,
      eventData.delta.y,
      0);// / transform.lossyScale.x; // Thanks to the canvas scaler we need to devide pointer delta by canvas scale to match pointer movement.

    transform.SetParent(canvas.transform, true);
    transform.SetAsLastSibling();
  }

  public void OnDrag(PointerEventData eventData)
  {
    transform.localPosition += new Vector3(
      eventData.delta.x,
      eventData.delta.y,
      0);// / transform.lossyScale.x; // Thanks to the canvas scaler we need to devide pointer delta by canvas scale to match pointer movement.
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    var results = new List<RaycastResult>();
    raycaster.Raycast(eventData, results);

    // Get drag destination.
    CampLocation newLocation = null;
    foreach (RaycastResult result in results)
    {
      newLocation = result.gameObject.GetComponent<CampLocation>();
      if (newLocation != null) break;
    }

    // If destination can accept this, move there.
    if (newLocation != null && newLocation.CanAccept(this))
      MoveTo(newLocation);
    // If destination has a fallback which can accept this, move there.
    else if (newLocation?.fallbackLocation != null && newLocation.fallbackLocation.CanAccept(this))
      MoveTo(newLocation.fallbackLocation);
    // Otherwise, cancel move.
    else
      location.CancelMove(this);

    dragInProgress = false;
    portrait.raycastTarget = true;
    highlight.raycastTarget = true;
    nameText.raycastTarget = true;
  }

  public void Select()
  {
    if (selected != null)
      selected.Deselect();
    selected = this;
    highlight.enabled = true;
    if (hero.action == null)
      location.ShowActions();
    StatsPanel.ShowStatsFor(hero);
  }

  public void Deselect()
  {
    highlight.enabled = false;
    if (selected == this)
      selected = null;
  }

  private void MoveTo(CampLocation newLocation)
  {
    location.Remove(this);
    newLocation.Add(this);
    location = newLocation;
  }

  public void SelectAction(CampAction action)
  {
    bool actionSelected = action != null;

    hero.action = action;
    portrait.raycastTarget = !actionSelected;
    highlight.raycastTarget = !actionSelected;
    nameText.raycastTarget = !actionSelected;
    actionText.raycastTarget = !actionSelected;
    actionText.gameObject.SetActive(actionSelected);
    if (actionSelected)
      actionText.text = action.titlePresentProgressive;
    cancelButton.gameObject.SetActive(true);

    CampController.OnActionSelected(hero);
  }

  public void CancelAction() => SelectAction(null);

  public void AllowCancel(bool allowed)
  {
    UnityEngine.Debug.Log("ALLOW CANCEL " + allowed);
    cancelButton.gameObject.SetActive(allowed);
  }

  public void ClearActionText()
  {
    actionText.gameObject.SetActive(false);
  }
}
