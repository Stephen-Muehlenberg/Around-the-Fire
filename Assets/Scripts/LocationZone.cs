using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A UI area where <see cref="HeroPortrait"/>s can be dropped into to 
/// assign them to a <see cref="CampLocation"/>. Most Locations have only
/// one Zone, but it's possible to divide Heroes among multiple.
/// </summary>
public class LocationZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public HeroLocation location;
  public int maxHeroes;
  public List<HeroPortrait> heroes = new List<HeroPortrait>();
  [SerializeField] private Image highlight;
  [SerializeField] private Transform portraitParent;

  public void SetHighlighted(bool highlighted)
    => highlight.color = new Color(highlight.color.r, highlight.color.g, highlight.color.b, highlighted ? 0.1254f : 0f);

  public void OnPointerEnter(PointerEventData eventData)
    => location.OnPointerEnter();

  public void OnPointerExit(PointerEventData eventData)
    => location.OnPointerExit();

  /// <summary>
  /// Can the <paramref name="hero"/> move to the parent <see cref="HeroLocation"/>?
  /// False if the Hero is already in this <see cref="LocationZone"/>.
  /// </summary>
  public bool CanAccept(HeroPortrait hero)
  {
    // Can't move to current Zone.
    if (heroes.Contains(hero)) return false;

    // Can only move to the same Location if moving to a different vacant Zone.
    if (location.heroes.Contains(hero))
    {
      return location.zones.Where(it => !it.heroes.Contains(hero))
        .Any(it => it.heroes.Count < it.maxHeroes);
    }
    // If moving to a new Location, at least one Zone must be vacant.
    else
    {
      return location.zones.Any(it => it.heroes.Count < it.maxHeroes);
    }
  }

  public void Add(HeroPortrait hero, bool showActions = true)
  {
    if (!CanAccept(hero))
      throw new System.Exception("Can't accept hero '" + hero.hero.name + "' at " + location.name + "." + this.name + ".");

    // If this Zone is full, overflow to another.
    if (heroes.Count == maxHeroes)
    {
      location.zones
        .First(it => it.heroes.Count < it.maxHeroes)
        .Add(hero);
      return;
    }

    heroes.Add(hero);
    hero.transform.SetParent(portraitParent);
    location.Add(hero, showActions);
  }

  public void ReturnToPreviousPosition(HeroPortrait hero)
  {
    int index = heroes.IndexOf(hero);
    hero.transform.SetParent(portraitParent);
    hero.transform.SetSiblingIndex(index);
  }
}
