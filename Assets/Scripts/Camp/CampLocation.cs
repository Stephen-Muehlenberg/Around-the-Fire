using System.Linq;
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
  public int maxAdventurers;
  /// <summary>If this location is full, new adventurers are added to the fallback instead.</summary>
  public CampLocation fallbackLocation;

  [SerializeField]
  private List<AdventurerPortrait> adventurers = new List<AdventurerPortrait>();

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
    foreach (AdventurerPortrait adventurer in adventurers)
      adventurer.location = this;
    foreach (Image image in imagesToHighlight)
      image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (!AdventurerPortrait.dragInProgress) return;

    foreach (Image image in imagesToHighlight)
      image.color = new Color(image.color.r, image.color.g, image.color.b, 0.125f);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    foreach (Image image in imagesToHighlight)
      image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
  }

  public bool CanAccept(AdventurerPortrait adventurer)
    => adventurers.Count < maxAdventurers
      && !adventurers.Contains(adventurer);

  public void Add(AdventurerPortrait adventurer)
  {
    if (!CanAccept(adventurer))
      throw new System.Exception("Can't accept adventurer at " + this.name + ".");

    adventurers.Add(adventurer);
    adventurer.transform.SetParent(portraitParent);
    ShowActions();
  }

  public void CancelMove(AdventurerPortrait adventurer)
  {
    if (!adventurers.Contains(adventurer))
      throw new System.Exception("Can't return adventurer to " + this.name + " - not listed at this location.");

    int index = adventurers.IndexOf(adventurer);
    adventurer.transform.SetParent(portraitParent);
    adventurer.transform.SetSiblingIndex(index);
    ShowActions();
  }

  public void Remove(AdventurerPortrait adventurer)
    => adventurers.Remove(adventurer);

  public void ShowActions()
  {
    ActionList.Show(actions, OnActionSelected);
  }

  private void OnActionSelected(CampAction action)
  {
    if (AdventurerPortrait.selected == null)
      throw new System.Exception("Cannot select an action when no adventurers selected!");

    AdventurerPortrait.selected.SelectAction(action);
  }
}
