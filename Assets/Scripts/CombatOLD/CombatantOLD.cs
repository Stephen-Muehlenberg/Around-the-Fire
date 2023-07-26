using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A participant in a <see cref="Combat"/>. Either a <see cref="HeroCombatant"/>
/// or a <see cref="EnemyCombatantOLD"/>.
/// </summary>
public abstract class CombatantOLD
{
  abstract public bool isHero { get; }
  abstract public bool isEnemy { get; }
  abstract public string name { get; }

  public Portrait portrait;
  public CombatPortrait combatPortrait;

  public float health;
  public float healthMax;
  /// <summary>Normal maximum health, before any kind of modifiers to max health.</summary>
  public float healthBase;

  public float block;
  public float blockMax;

  /// <summary>0 = front line, 1 = second from front, etc.</summary>
  public int position;
  public bool hasActedThisRound;
  public CombatAction action;
  public CombatAction previousAction;

  public float attack => attackBase; // TODO Add attack modifiers here.
  abstract public float attackBase { get; }

  public float defense => defenseBase; // TODO Add defense modifiers here.
  abstract public float defenseBase { get; }

  public abstract void SetAction(CombatAction action);

  public void TakeDamage(int damage, (string, Color)? message = null)
  {
    float blockDamage = damage >= block ? block : damage;
    float healthDamage = damage - blockDamage;
    int nextMessageOffset = 0;

    if (blockDamage > 0)
    {
      block -= blockDamage;
      if (block <= 0)
      {
        block = 0;
        action = new CombatAction.Stagger()
        {
        //  origin = this
        };
      }
      combatPortrait.SetCondition(block / blockMax);
      StatPopup.Show(portrait, blockDamage.ToString(), Color.red);
      nextMessageOffset++;
    }

    if (healthDamage > 0)
    {
      health -= damage;
      if (health < 0) health = 0;
      combatPortrait.SetHealth(health / healthMax);
      StatPopup.Show(portrait, damage.ToString(), Color.red, nextMessageOffset);
      nextMessageOffset++;
    }

    if (message != null)
      StatPopup.Show(portrait, 
        text: message.Value.Item1,
        color: message.Value.Item2,
        countOffset: nextMessageOffset);
  }

  abstract public void SetPortrait(Portrait portrait, Combat.PortraitCallbacks callbacks);

  abstract public CombatantActionOLD ChooseAction(List<CombatantOLD> heroes, List<CombatantOLD> enemies);
  abstract public CombatAction GetAction(Combat state);

  public void TakeHit(float attackSuccess)
  {
    if (attackSuccess > 0)
      RegularHit();
    else
      GlancingBlow();
  }

  private void RegularHit()
  {
    health -= 5;
    StatPopup.Show(portrait, "HIT!", color: Color.red, countOffset: 2);
    StatPopup.Show(portrait, Hero.Stat.HEALTH, -5, countOffset: 1);
  }

  private void GlancingBlow()
  {
    //      armour--;
    StatPopup.Show(portrait, "BLOCK!", color: Color.cyan, countOffset: 2);
    StatPopup.Show(portrait, "Def -5", color: Color.cyan, countOffset: 1);
  }
}
