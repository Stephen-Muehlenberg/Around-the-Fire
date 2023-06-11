using UnityEngine;
using UnityEngine.UI;

public class CombatPortrait : MonoBehaviour
{
  private static readonly Color defenseFillPositive = new Color(0.754717f, 0.6835152f, 0.5233179f);
  private static readonly Color defenseFillNegative = new Color(0.5754717f, 0.122152f, 0.122152f);

  [SerializeField] private Slider healthBar;
  [SerializeField] private Slider conditionBar;
  [SerializeField] private Image defenseModifierIcon;
  [SerializeField] private TMPro.TMP_Text defenseModifierText;

  public void Initialise(float? health = null, float? condition = null, float? defenseModifier = null)
  {
    if (health.HasValue)
      healthBar.value = health.Value;
    healthBar.gameObject.SetActive(health.HasValue);

    if (condition.HasValue)
      conditionBar.value = condition.Value;
    conditionBar.gameObject.SetActive(condition.HasValue);

    if (defenseModifier.HasValue)
      SetDefenseModifier(defenseModifier.Value);
    this.defenseModifierIcon.gameObject.SetActive(defenseModifier.HasValue);
  }

  public void ShowHealthBar(bool show) => healthBar.gameObject.SetActive(show);
  public void SetHealth(float healthFraction) => healthBar.value = healthFraction;
  public void ShowConditionBar(bool show) => conditionBar.gameObject.SetActive(show);
  public void SetCondition(float conditionFraction) => conditionBar.value = conditionFraction;
  public void ShowDefenseBonus(bool show) => defenseModifierIcon.gameObject.SetActive(show);
  public void SetDefenseModifier(float defenseModifier)
  {
    if (defenseModifier > 0)
    {
      defenseModifierIcon.gameObject.SetActive(true);
      defenseModifierIcon.color = defenseFillPositive;
      defenseModifierText.text = "+" + Mathf.FloorToInt(defenseModifier);

    } else if (defenseModifier < 0)
    {
      defenseModifierIcon.gameObject.SetActive(true);
      defenseModifierIcon.color = defenseFillNegative;
      defenseModifierText.text = Mathf.FloorToInt(defenseModifier).ToString();
    } else
    {
      defenseModifierIcon.gameObject.SetActive(false);
    }
  }
}
