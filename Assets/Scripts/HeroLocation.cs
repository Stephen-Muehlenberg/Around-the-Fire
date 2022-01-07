using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Location where <see cref="HeroPortrait"/>s can be assigned.
/// </summary>
public class HeroLocation : MonoBehaviour
{
  public static HeroLocation Fire;
  public static HeroLocation Around;
  public static HeroLocation Clearing;
  public static HeroLocation Tent;
  public static HeroLocation Perimeter;
  public static HeroLocation Supplies;
  public static HeroLocation CharacterPanel;

  public List<LocationZone> zones;
  public List<HeroPortrait> heroes = new List<HeroPortrait>();

  private void Awake()
  {
    switch (name)
    {
      case "Fire": Fire = this; break;
      case "Around": Around = this; break;
      case "Tent": Tent = this; break;
      case "Supplies": Supplies = this; break;
      case "Clearing": Clearing = this; break;
      case "Forest": Perimeter = this; break;
      case "Character Panel": CharacterPanel = this; break;
    }
  }

  public void Start()
  {
    zones.ForEach(zone =>
    {
      zone.location = this;
      zone.SetHighlighted(false);
    });
  }

  public void OnPointerEnter()
  {
    if (!HeroPortrait.dragInProgress) return;
    zones.ForEach(it => it.SetHighlighted(true));
  }

  public void OnPointerExit()
  {
    zones.ForEach(it => it.SetHighlighted(false));
  }

  public void Add(HeroPortrait hero)
  {
    heroes.Add(hero);
    ShowActions(hero);
  }

  public void CancelMove(HeroPortrait hero)
  {
    zones.First(it => it.heroes.Contains(hero))
      .ReturnToPreviousPosition(hero);
    ShowActions(hero);
  }

  public void Remove(HeroPortrait hero)
  {
    heroes.Remove(hero);
    zones.ForEach(it => it.heroes.Remove(hero));
  }

  public void ShowActions(HeroPortrait hero)
  {
    var actions = ActionManager.GetActionsFor(this)
      // TODO Show unavailable but not hidden actions differently.
      .Where(it => it.AvailableFor(hero.hero, CampController.singleton.campState) == HeroAction.Availability.AVAILABLE)
      .ToList();
    ActionList.Show(actions, OnActionSelected);
  }

  private void OnActionSelected(HeroAction action)
  {
    if (HeroPortrait.selected == null)
      throw new System.Exception("Cannot select an action when no heroes selected!");

    HeroPortrait.selected.SelectAction(action);
  }
}
