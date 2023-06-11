using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StatusEffect = Combat.StatusEffect;

/// <summary>
/// Enemy <see cref="Combatant"/>s. See <see cref="HeroCombatant"/> for the player-controlled
/// counterpart.
/// </summary>
public class EnemyCombatant : Combatant
{
  public EnemyArchetype archetype;
  public override bool isHero => false;
  public override bool isEnemy => true;
  public override string name => archetype.name;
  public override int attackBase => archetype.attack;
  public override int defenseBase => archetype.defense;

  public EnemyCombatant(EnemyArchetype archetype)
  {
    this.archetype = archetype;
    health = archetype.health;
  }

  public override void SetPortrait(Portrait portrait)
  {
    this.portrait = portrait;
    var icon = Resources.Load<Sprite>("Icons/Enemies/" + archetype.icon);
    var enemyChar = new Character()
    {
      name = archetype.name,
      icon = icon
    };
    portrait.Initialise(enemyChar);
    combatPortrait = portrait.GetComponent<CombatPortrait>();
    combatPortrait.Initialise(
      health: 1,
      condition: null,
      defenseModifier: null);
  }

  public override CombatantAction GetAction(List<Combatant> heroes, List<Combatant> enemies)
  {
    return new CombatantAction.Attack()
    {
      origin = this,
      target = heroes.Random()
    };
  }
}