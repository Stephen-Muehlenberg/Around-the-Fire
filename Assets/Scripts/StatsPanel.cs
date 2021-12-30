using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
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

  private static StatsPanel singleton;

  void Awake()
  {
    if (singleton != null) throw new System.Exception("Cannot have two StatsPanel singletons.");
    singleton = this;
  }

  void Start()
  {
    ShowStatsFor(null);
  }

  public static void ShowStatsFor(Adventurer adventurer)
  {
    singleton.gameObject.SetActive(adventurer != null);
    if (adventurer == null) return;

    singleton.health.value = adventurer.health;
    singleton.healthFill.color = singleton.fillColour.Evaluate(adventurer.health / 100f);
    singleton.hunger.value = adventurer.hunger;
    singleton.hungerFill.color = singleton.fillColour.Evaluate(adventurer.hunger / 100f);
    singleton.rest.value = adventurer.rest;
    singleton.restFill.color = singleton.fillColour.Evaluate(adventurer.rest / 100f);
    singleton.mood.value = adventurer.mood;
    singleton.moodFill.color = singleton.fillColour.Evaluate(adventurer.mood / 100f);
  }
}
