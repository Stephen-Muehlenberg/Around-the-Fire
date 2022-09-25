using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI representation of a <see cref="Hero"/>.
/// </summary>
public class HeroPortrait : MonoBehaviour,
  IBeginDragHandler, IDragHandler, IEndDragHandler,
  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
  private static Color COLOR_HIGHLIGHTED = new Color(1, 0.9227282f, 0, 0.2784314f);
  private static Color COLOR_SELECTED = new Color(0.2293994f, 1, 0, 0.2784314f);

  public Hero hero { get; private set; }
//  public HeroZoneUi location { get => hero.location; set { hero.location = value; } }
  public EventsCallback callback;

  [SerializeField] private Image portrait;
  [SerializeField] private Image highlight;
  [SerializeField] private TMPro.TMP_Text nameText;
  [SerializeField] private TMPro.TMP_Text actionText;
  [SerializeField] private Button cancelButton;
  /*
  public void Initialise(Hero hero, ZoneUi location, EventsCallback eventCallback)
  {
    callback = eventCallback;
    this.hero = hero;
    nameText.text = hero.name;
    portrait.sprite = hero.icon;
//    this.location = location;
  //  if (location != null)
  //    location.subZones[0].Add(portrait: this, showActions: false);
    ShowSelectedAction(null);
  }
  */
  public void Highlight()
  {
    highlight.enabled = true;
//    highlight.color = (hero == CampScene.selectedHero) ? COLOR_SELECTED : COLOR_HIGHLIGHTED;
  }

  public void Unhighlight()
  {
//    highlight.enabled = (hero == CampScene.selectedHero);
  }

  public void Select()
  {
    highlight.enabled = true;
    highlight.color = COLOR_SELECTED;
  }

  public void Deselect()
  {
    highlight.enabled = false;
  }

  /// <summary>
  /// True if <see cref="HeroPortrait"/> should block raycasts.
  /// </summary>
  public void SetRaycastTarget(bool enabled)
  {
    portrait.raycastTarget = enabled;
    highlight.raycastTarget = enabled;
    nameText.raycastTarget = enabled;
  }
  /*
  public void MoveTo(SubzoneUi newZone, bool showActions = true)
  {
//    if (location != null) location.Remove(this);
  //  newZone.Add(this, showActions);
 //   location = newZone.location;
  }*/
  /*
  public IEnumerator AnimateMoveTo(ZoneUi location)
  {
    var zone = location.subZones
      .First(it => it.heroes.Count < it.maxHeroes);
    MoveTo(newZone: zone, showActions: false);
    // TODO animate.
    yield return new WaitForSeconds(0.2f);
  }*/

  public void ShowSelectedAction(HeroAction action)
  {
    bool actionSelected = action != null;
    actionText.gameObject.SetActive(actionSelected);
    if (actionSelected)
      actionText.text = action.titlePresentProgressive;
    cancelButton.gameObject.SetActive(actionSelected);
  }

  public void AllowCancel(bool allowed)
  {
    cancelButton.gameObject.SetActive(allowed);
  }

  public void ClearActionText()
  {
    actionText.gameObject.SetActive(false);
  }

  // HeroPortrait exposes various UI events for different scenes to 
  // consume in different ways.
  public interface EventsCallback
  {
    public void OnPointerEnterPortrait(HeroPortrait portrait);
    public void OnPointerExitPortrait(HeroPortrait portrait);
    public void OnPointerClickPortrait(HeroPortrait portrait);
    public void OnPortraitDragStart(HeroPortrait portrait, PointerEventData data);
    public void OnPotraitDrag(HeroPortrait portrait, PointerEventData data);
    public void OnPotraitDragEnd(HeroPortrait portrait, PointerEventData data);
    public void OnPortaitCancelPressed(HeroPortrait portrait);
  }

  public void OnPointerEnter(PointerEventData eventData)
    => callback?.OnPointerEnterPortrait(this);
  public void OnPointerExit(PointerEventData eventData)
    => callback?.OnPointerExitPortrait(this);
  public void OnPointerClick(PointerEventData eventData)
    => callback?.OnPointerClickPortrait(this);
  public void OnBeginDrag(PointerEventData eventData)
    => callback?.OnPortraitDragStart(this, eventData);
  public void OnDrag(PointerEventData eventData)
    => callback?.OnPotraitDrag(this, eventData);
  public void OnEndDrag(PointerEventData eventData)
    => callback?.OnPotraitDragEnd(this, eventData);
  public void CancelAction()
    => callback?.OnPortaitCancelPressed(this);
}
