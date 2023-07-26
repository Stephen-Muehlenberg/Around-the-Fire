using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatZonesUi : MonoBehaviour
{
  [SerializeField] private LayoutGroup heroBackline;
  [SerializeField] private LayoutGroup heroFrontline;
  [SerializeField] private LayoutGroup enemyFrontline;
  [SerializeField] private LayoutGroup enemyBackline;

  public void Initialise(
    List<CombatPortrait> backlineHeroes,
    List<CombatPortrait> frontlineHeroes,
    List<CombatPortrait> frontlineEnemies,
    List<CombatPortrait> backlineEnemies)
  {
    // If there's placeholder elements in the layout groups, remove them.
    InitialiseZone(heroBackline, backlineHeroes);
    InitialiseZone(heroFrontline, frontlineHeroes);
    InitialiseZone(enemyFrontline, frontlineEnemies);
    InitialiseZone(enemyBackline, backlineEnemies);
  }

  private void InitialiseZone(LayoutGroup zone, List<CombatPortrait> combatantPortraits)
  {
    // Remove any existing portraits or placeholder images.
    for (int i = zone.transform.childCount - 1; i >= 0; i--)
      Destroy(zone.transform.GetChild(i).gameObject);
    
    for (int i = 0; i < combatantPortraits.Count; i++)
      combatantPortraits[i].transform.parent = zone.transform;
  }

  public void SetHeroPosition(CombatPortrait portrait, bool frontRow)
    => SetCombatantPosition(portrait, frontRow ? heroFrontline : heroBackline);

  public void SetEnemyPosition(CombatPortrait portrait, bool frontRow)
    => SetCombatantPosition(portrait, frontRow ? enemyFrontline : enemyBackline);

  private void SetCombatantPosition(CombatPortrait portrait, LayoutGroup zone)
  {
    portrait.transform.parent = zone.transform;
  }

  public void RemovePortrait(CombatPortrait portrait)
  {
    portrait.transform.parent = null;
    Destroy(portrait.gameObject);
  }
}
