using UnityEngine;
using UnityEngine.UI;

public class HeroStatsPanel : MonoBehaviour
{
  public Slider health;
  public Image healthFill;
  public Slider hunger;
  public Image hungerFill;
  public Slider rest;
  public Image restFill;
  public Slider mood;
  public Image moodFill;
  public Gradient fillColour;

  private static HeroStatsPanel singleton;

  void Awake()
  {
    if (singleton != null) throw new System.Exception("Cannot have two StatsPanel singletons.");
    singleton = this;
  }

  void Start()
  {
    ShowStatsFor(null);
  }

  public static void ShowStatsFor(Hero hero)
  {
    singleton.gameObject.SetActive(hero != null);
    if (hero == null) return;

    singleton.health.value = hero.health;
    singleton.healthFill.color = singleton.fillColour.Evaluate(hero.health / 100f);
    singleton.hunger.value = hero.hunger;
    singleton.hungerFill.color = singleton.fillColour.Evaluate(hero.hunger / 100f);
    singleton.rest.value = hero.rest;
    singleton.restFill.color = singleton.fillColour.Evaluate(hero.rest / 100f);
    singleton.mood.value = hero.mood;
    singleton.moodFill.color = singleton.fillColour.Evaluate(hero.mood / 100f);
  }
}
