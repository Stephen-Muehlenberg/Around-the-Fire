using System.Collections.Generic;
using UnityEngine;

using StatusEffect = Combat.StatusEffect;

/// <summary>
/// <see cref="Combatant"/>-specific logic for a <see cref="Hero"/>. See <see cref="EnemyCombatant"/> for enemy counterpart.
/// </summary>
public class HeroCombatant : Combatant
{
  public Hero hero;
  public override bool isHero => true;
  public override bool isEnemy => false;
  public override string name => hero.name;
  public override int attackBase => Mathf.FloorToInt(hero.totalSkill);
  public override int defenseBase => Mathf.FloorToInt(hero.totalSkill);

  public HeroCombatant(Hero hero)
  {
    this.hero = hero;
    health = hero.health;
  }

  public override void SetPortrait(Portrait portrait)
  {
    this.portrait = portrait;
    portrait.Initialise(hero, Portrait.Interactions.CLICKABLE);
    combatPortrait = portrait.GetComponent<CombatPortrait>();
    combatPortrait.Initialise(
      health: hero.health / 100f,
      condition: hero.totalSkill / 100f,
      defenseModifier: null);
  }

  public override CombatantAction GetAction(List<Combatant> heroes, List<Combatant> enemies)
  {
    return new CombatantAction.Attack()
    {
      origin = this,
      target = enemies.Random()
    };
  }
}
