using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampUi : MonoBehaviour
{
  [SerializeField] private HeroStatsPanel heroStatsPanel;

  public void ShowStatsFor(Hero hero)
  {
    heroStatsPanel.ShowStatsFor(hero);
  }

  public void ShowPartyStats()
  {
    heroStatsPanel.ShowStatsFor(null);
  }
}
