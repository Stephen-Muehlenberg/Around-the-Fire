using System.Linq;
using System.Collections;
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
  private static Color COLOR_HIGHLIGHTED = new Color(1, 0.9227282f, 0, 0.2784314f);
  private static Color COLOR_SELECTED = new Color(0.2293994f, 1, 0, 0.2784314f);

  public Hero hero { get; private set; }
  public HeroLocation location { get => hero.location; set { hero.location = value; } }

  private Canvas canvas;
  private GraphicRaycaster raycaster;
  [SerializeField] private Image portrait;
  [SerializeField] private Image highlight;
  [SerializeField] private TMPro.TMP_Text nameText;
  [SerializeField] private TMPro.TMP_Text actionText;
  [SerializeField] private Button cancelButton;

  public void Initialise(Hero hero, HeroLocation location)
  {
    this.hero = hero;
    nameText.text = hero.name;
    portrait.sprite = hero.icon;
    this.location = location;
    location.zones[0].Add(hero: this, showActions: false);
    canvas = GetComponentInParent<Canvas>();
    raycaster = GetComponentInParent<GraphicRaycaster>();
    ShowSelectedAction(null);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (CampController.uiState != CampController.UIState.INTERACTIVE) return;
    Highlight();
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (CampController.uiState != CampController.UIState.INTERACTIVE) return;
    Unhighlight();
  }

  public void Highlight()
  {
    highlight.enabled = true;
    highlight.color = (hero == CampController.selectedHero) ? COLOR_SELECTED : COLOR_HIGHLIGHTED;
    HeroStatsPanel.ShowStatsFor(hero);
  }

  public void Unhighlight()
  {
    var selected = CampController.selectedHero;
    highlight.enabled = (hero == selected);
    HeroStatsPanel.ShowStatsFor((selected == null) ? null : selected);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (CampController.uiState != CampController.UIState.INTERACTIVE) return;
    Select();
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    if (CampController.uiState != CampController.UIState.INTERACTIVE) return;

    CampController.uiState = CampController.UIState.DRAG_IN_PROCESS;
    portrait.raycastTarget = false;
    highlight.raycastTarget = false;
    nameText.raycastTarget = false;

    Select(showActions: false);

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

    portrait.raycastTarget = true;
    highlight.raycastTarget = true;
    nameText.raycastTarget = true;
    CampController.uiState = CampController.UIState.INTERACTIVE;
  }

  public void Select(bool showActions = true)
  {
    if (CampController.selectedHero != null)
      CampController.selectedHero.portrait.Deselect();
    CampController.selectedHero = hero;
    highlight.enabled = true;
    highlight.color = COLOR_SELECTED;
    if (hero.action == null && showActions)
      location.ShowActions(hero);
    else
      location.ShowActions(null);
    HeroStatsPanel.ShowStatsFor(hero);
  }

  public void Deselect()
  {
    highlight.enabled = false;
    if (CampController.selectedHero == hero)
      CampController.selectedHero = null;
  }

  private void MoveTo(LocationZone newZone, bool showActions = true)
  {
    location.Remove(this);
    newZone.Add(this, showActions);
    location = newZone.location;
  }

  public IEnumerator AnimateMoveTo(HeroLocation location)
  {
    var zone = location.zones
      .First(it => it.heroes.Count < it.maxHeroes);
    MoveTo(newZone: zone, showActions: false);
    // TODO animate.
    yield return new WaitForSeconds(0.2f);
  }

  public void ShowSelectedAction(HeroAction action)
  {
    bool actionSelected = action != null;
    actionText.gameObject.SetActive(actionSelected);
    if (actionSelected)
      actionText.text = action.titlePresentProgressive;
    cancelButton.gameObject.SetActive(actionSelected);
  }

  public void CancelAction()
  {
    hero.SelectAction(null);
    if (CampController.selectedHero == hero)
      location.ShowActions(hero);
  }

  public void AllowCancel(bool allowed)
  {
    cancelButton.gameObject.SetActive(allowed);
  }

  public void ClearActionText()
  {
    actionText.gameObject.SetActive(false);
  }
}
