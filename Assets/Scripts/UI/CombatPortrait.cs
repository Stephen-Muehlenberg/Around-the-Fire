using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CombatPortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
  private static readonly Color defenseFillPositive = new Color(0.754717f, 0.6835152f, 0.5233179f);
  private static readonly Color defenseFillNegative = new Color(0.5754717f, 0.122152f, 0.122152f);

  [SerializeField] private Slider healthBar;
  [SerializeField] private Slider conditionBar;
  [SerializeField] private Image defenseModifierIcon;
  [SerializeField] private TMPro.TMP_Text defenseModifierText;

  private Combatant combatant;
  private Combat.PortraitCallbacks callbacks;

  public void Initialise(Combatant combatant, Combat.PortraitCallbacks callbacks, float? health = null, float? condition = null, float? defenseModifier = null)
  {
    this.combatant = combatant;

    if (health.HasValue)
      healthBar.value = health.Value;
    healthBar.gameObject.SetActive(health.HasValue);

    if (condition.HasValue)
      conditionBar.value = condition.Value;
    conditionBar.gameObject.SetActive(condition.HasValue);

    if (defenseModifier.HasValue)
      SetDefenseModifier(defenseModifier.Value);
    this.defenseModifierIcon.gameObject.SetActive(defenseModifier.HasValue);

    this.callbacks = callbacks;
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

  public void OnPointerClick(PointerEventData eventData)
    => callbacks.OnClick(combatant);

  public void OnPointerExit(PointerEventData eventData)
    => callbacks.OnHoverExit(combatant);

  public void OnPointerEnter(PointerEventData eventData)
    => callbacks.OnHoverEnter(combatant);
}
