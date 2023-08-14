using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A <see cref="Portrait"/> variant which shows details unique to <see cref="Hero"/>s.
/// </summary>
public class HeroPortrait : Portrait
{
  [SerializeField] private Slider statusSlider;

  public Hero hero { get; private set; }

  public void Initialise(
    Hero hero,
    Interactions interactions = Interactions.NONE,
    EventsCallback callbacks = null)
  {
    base.Initialise(hero, interactions, callbacks);
    this.hero = hero;
    statusSlider.value = hero.totalSkill / 100f;
    hero.statusChanges += OnHeroStatusChanged;
  }

  private void OnDestroy()
  {
    if (hero != null)
      hero.statusChanges -= OnHeroStatusChanged;
  }

  private void OnHeroStatusChanged(Hero hero)
  {
    statusSlider.value = hero.totalSkill / 100f;
  }

  public void SetStatus(float status) => statusSlider.value = status;
}
