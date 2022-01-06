using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI representation of a <see cref="Hero"/>. Can be dragged
/// around to different <see cref="HeroLocation"/>s.
/// </summary>
public class HeroPortrait : MonoBehaviour,
  IBeginDragHandler, IDragHandler, IEndDragHandler,
  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
  public Hero hero { get; private set; }
  public HeroLocation location { get => hero.location; set { hero.location = value; } }

  private Canvas canvas;
  private GraphicRaycaster raycaster;
  [SerializeField] private Image portrait;
  [SerializeField] private Image highlight;
  [SerializeField] private TMPro.TMP_Text nameText;
  [SerializeField] private TMPro.TMP_Text actionText;
  [SerializeField] private Button cancelButton;

  public static HeroPortrait selected;
  public static bool dragInProgress; // Don't highlight heroes while dragging.

  public void Initialise(Hero hero, HeroLocation location)
  {
    this.hero = hero;
    nameText.text = hero.name;
    portrait.sprite = hero.icon;
    this.location = location;
    location.zones[0].Add(this);
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
    LocationZone newZone = null;
    foreach (RaycastResult result in results)
    {
      newZone = result.gameObject.GetComponentInParent<LocationZone>();
      if (newZone != null) break;
    }

    // If destination can accept this, move there.
    if (newZone != null && newZone.CanAccept(this))
      MoveTo(newZone);
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
      location.ShowActions(this);
    StatsPanel.ShowStatsFor(hero);
  }

  public void Deselect()
  {
    highlight.enabled = false;
    if (selected == this)
      selected = null;
  }

  private void MoveTo(LocationZone newZone)
  {
    location.Remove(this);
    newZone.Add(this);
    location = newZone.location;
  }

  public void SelectAction(HeroAction action)
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
