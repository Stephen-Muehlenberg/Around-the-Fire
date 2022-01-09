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
    if (CampController.uiState != CampController.UIState.DRAG_IN_PROCESS) return;
    zones.ForEach(it => it.SetHighlighted(true));
  }

  public void OnPointerExit()
  {
    zones.ForEach(it => it.SetHighlighted(false));
  }

  public bool HasSpace()
    => zones.Any(it => it.heroes.Count < it.maxHeroes);

  public void Add(HeroPortrait hero, bool showActions = true)
  {
    heroes.Add(hero);
    if (showActions) ShowActions(hero);
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
    if (hero == null)
    {
      ActionList.Hide();
      return;
    }

    var actions = ActionManager.GetActionsFor(this)
      // TODO Show unavailable but not hidden actions differently.
      .Where(it => it.AvailableFor(hero.hero, CampController.campState) == HeroAction.Availability.AVAILABLE)
      .ToList();
    ActionList.Show(actions, OnActionSelected);
  }

  private void OnActionSelected(HeroAction action)
  {
    if (CampController.selectedHero == null)
      throw new System.Exception("Cannot select an action when no heroes selected!");

    CampController.selectedHero.SelectAction(action);
  }
}
