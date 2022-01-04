using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampLocation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public static CampLocation Fire;
  public static CampLocation Around;
  public static CampLocation Clearing;
  public static CampLocation Tent;
  public static CampLocation Forest;
  public static CampLocation Supplies;
  public static CampLocation CharacterPanel;

  public List<CampAction> actions;
  public Image[] imagesToHighlight;
  public Transform portraitParent;
  public int maxHeroes;
  /// <summary>If this location is full, new heroes are added to the fallback instead.</summary>
  public CampLocation fallbackLocation;

  [SerializeField]
  private List<HeroPortrait> heroes = new List<HeroPortrait>();

  private void Awake()
  {
    switch (name)
    {
      case "Fire": Fire = this; break;
      case "Around": Around = this; break;
      case "Tent": Tent = this; break;
      case "Supplies": Supplies = this; break;
      case "Clearing": Clearing = this; break;
      case "Forest": Forest = this; break;
      case "Character Panel": CharacterPanel = this; break;
    }
  }

  public void Start()
  {
    foreach (HeroPortrait hero in heroes)
      hero.location = this;
    foreach (Image image in imagesToHighlight)
      image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (!HeroPortrait.dragInProgress) return;

    foreach (Image image in imagesToHighlight)
      image.color = new Color(image.color.r, image.color.g, image.color.b, 0.125f);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    foreach (Image image in imagesToHighlight)
      image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
  }

  public bool CanAccept(HeroPortrait hero)
    => heroes.Count < maxHeroes
      && !heroes.Contains(hero);

  public void Add(HeroPortrait hero)
  {
    if (!CanAccept(hero))
      throw new System.Exception("Can't accept hero at " + this.name + ".");

    heroes.Add(hero);
    hero.transform.SetParent(portraitParent);
    ShowActions();
  }

  public void CancelMove(HeroPortrait hero)
  {
    if (!heroes.Contains(hero))
      throw new System.Exception("Can't return hero to " + this.name + " - not listed at this location.");

    int index = heroes.IndexOf(hero);
    hero.transform.SetParent(portraitParent);
    hero.transform.SetSiblingIndex(index);
    ShowActions();
  }

  public void Remove(HeroPortrait hero)
    => heroes.Remove(hero);

  public void ShowActions()
  {
    ActionList.Show(actions, OnActionSelected);
  }

  private void OnActionSelected(CampAction action)
  {
    if (HeroPortrait.selected == null)
      throw new System.Exception("Cannot select an action when no heroes selected!");

    HeroPortrait.selected.SelectAction(action);
  }
}
