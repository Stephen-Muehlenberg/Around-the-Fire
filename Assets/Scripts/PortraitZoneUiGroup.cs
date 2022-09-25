using System.Collections;
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
    public void OnRightClickZone(PortraitZoneUiGroup zoneUi, PortraitZoneUiArea childClicked);
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

  public void Add(Portrait portrait)
  {
    if (!CanAccept(portrait))
      throw new System.Exception("Can't add portrait; not enough space!");

    portraits.Add(portrait);
    childAreas.First(it => it.CanAccept(portrait))
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
    if (data.button != PointerEventData.InputButton.Right) return;
    if (callback != null)
      callback.OnRightClickZone(this, childArea);
  }

  /*

  public void ShowActions(Hero hero, ActionList actionList)
  {
    if (hero == null)
    {
      actionList.Hide();
      return;
    }

    var actions = ActionManager.GetCampActionsFor(zone);
    var actionButtons = actions
      .Select(it => new ActionButton.Content() {
        text = it.title,
        hoverText = it.description,
        state = (int) it.AvailableFor(hero, Game.state)
      })
      .ToList();
    actionList.Show(actionButtons, (i) => OnActionSelected(actions[i]));
  }

  private void OnActionSelected(HeroAction action)
  {
//    if (CampScene.selectedHero == null)
      throw new System.Exception("Cannot select an action when no heroes selected!");

//    CampScene.selectedHero.SelectAction(action);
  }
   */
}
