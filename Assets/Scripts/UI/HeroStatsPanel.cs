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

  private Hero currentHero;

  void Start()
  {
    ShowStatsFor(null);
  }

  private void OnDestroy()
  {
    if (currentHero != null)
      currentHero.statusChanges -= UpdateStats;
  }

  public void ShowStatsFor(Hero hero)
  {
    if (currentHero != null)
      currentHero.statusChanges -= UpdateStats;
    currentHero = hero;
    if (hero != null)
      hero.statusChanges += UpdateStats;
    UpdateStats(hero);
  }

  private void UpdateStats(Hero hero)
  {
    gameObject.SetActive(hero != null);
    if (hero == null) return;

    health.value = hero.health;
    healthFill.color = fillColour.Evaluate(hero.health / 100f);
    hunger.value = hero.hunger;
    hungerFill.color = fillColour.Evaluate(hero.hunger / 100f);
    rest.value = hero.rest;
    restFill.color = fillColour.Evaluate(hero.rest / 100f);
    mood.value = hero.mood;
    moodFill.color = fillColour.Evaluate(hero.mood / 100f);
  }
}
