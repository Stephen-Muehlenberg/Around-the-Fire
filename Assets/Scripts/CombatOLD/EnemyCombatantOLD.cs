using System.Collections.Generic;
using UnityEngine;

using StatusEffect = CombatOLD.StatusEffect;

/// <summary>
/// Enemy <see cref="CombatantOLD"/>s. See <see cref="HeroCombatantOLD"/> for the player-controlled
/// counterpart.
/// </summary>
public class EnemyCombatantOLD : CombatantOLD
{
  public EnemyArchetype archetype;
  public override bool isHero => false;
  public override bool isEnemy => true;
  public override string name => archetype.name;
  public override float attackBase => archetype.attack;
  public override float defenseBase => archetype.defense;

  public EnemyCombatantOLD(EnemyArchetype archetype)
  {
    this.archetype = archetype;
    healthBase = archetype.health;
    healthMax = archetype.health;
    health = archetype.health;
    blockMax = archetype.block;
    block = archetype.block;
  }

  public override void SetPortrait(Portrait portrait, Combat.PortraitCallbacks callbacks)
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
   // combatPortrait.Initialise(
   //   this,
   //   callbacks,
   //   health: 1,
   //   condition: 1,
   //   defenseModifier: null);
  }

  public override void SetAction(CombatAction action)
  {
    this.action = action;
  }

  public override CombatantActionOLD ChooseAction(List<CombatantOLD> heroes, List<CombatantOLD> enemies)
  {
    return new CombatantActionOLD.Attack()
    {
      origin = this,
      target = heroes.Random()
    };
  }

  public override CombatAction GetAction(Combat state)
  {
    // TODO
    return new CombatAction.Attack()
    {
     // origin = this
    };
  }
}