using System.Collections.Generic;
using UnityEngine;

using StatusEffect = CombatOLD.StatusEffect;

/// <summary>
/// <see cref="Combatant"/>-specific logic for a <see cref="Hero"/>. See <see cref="EnemyCombatantOLD"/> for enemy counterpart.
/// </summary>
public class HeroCombatant : Combatant
{
  public Hero hero;
  public override bool isHero => true;
  public override bool isEnemy => false;
  public override string name => hero.name;
  public override float attackBase => hero.totalSkill; // TODO Add attack skill.
  public override float defenseBase => hero.totalSkill; // TODO Add defense skill.

  public HeroCombatant(Hero hero, int maxBlock = 100)
  {
    this.hero = hero;
    healthBase = 100;
    healthMax = 100;
    health = hero.health;
    blockMax = maxBlock;
    block = maxBlock;
  }

  public override void SetPortrait(Portrait portrait, Combat.PortraitCallbacks callbacks)
  {
    this.portrait = portrait;
    portrait.Initialise(hero, Portrait.Interactions.CLICKABLE);
    combatPortrait = portrait.GetComponent<CombatPortrait>();
    combatPortrait.Initialise(
      this,
      callbacks,
      health: hero.health / 100f,
      condition: hero.totalSkill / 100f,
      defenseModifier: null);
  }

  public override void SetAction(CombatAction action)
  {
    this.action = action;
    portrait.SetAction(action.name);
  }

  public override CombatAction ChooseAction(Combat state)
  {
    // TODO
    return new CombatAction.Attack()
    {
      origin = this
    };
  }

  public List<CombatAction> GetActions()
  {
    // TODO
    return new List<CombatAction>()
    {
      new CombatAction.Attack() { origin = this },
      new CombatActionsKnight.Shieldbreaker() { origin = this },
      new CombatAction.Attack() { origin = this },
    };
  }
}