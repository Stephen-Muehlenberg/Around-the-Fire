using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StatusEffect = Combat.StatusEffect;

/// <summary>
/// A participant in a <see cref="Combat"/>. Either a <see cref="HeroCombatant"/>
/// or a <see cref="EnemyCombatant"/>.
/// </summary>
public abstract class Combatant
{
  abstract public bool isHero { get; }
  abstract public bool isEnemy { get; }
  abstract public string name { get; }
  public Portrait portrait;
  public CombatPortrait combatPortrait;
  /// <summary>0 = front line, 1 = second from front, etc.</summary>
  public int position;
  public bool hasActedThisRound;
  public float health;

  public int attack => attackBase + attackModifiersSum;
  abstract public int attackBase { get; }
  public StatusEffect[] attackModifiers;
  private int attackModifiersSum;

  public int defense => defenseBase + defenseModifiersSum;
  abstract public int defenseBase { get; }
  public StatusEffect[] defenseModifiers;
  private int defenseModifiersSum;

  abstract public void SetPortrait(Portrait portrait);

  abstract public CombatantAction GetAction(List<Combatant> heroes, List<Combatant> enemies);

  public void TakeHit(float attackSuccess)
  {
    if (attackSuccess > 0)
    {
      if (attackSuccess >= 40)
        CriticalHit();
      else
      {
        /*          if (armour == 0) CriticalHit();
                  else*/
        RegularHit();
      }
    } else
    {
      if (attackSuccess > -40)
      {
        /*          if (armour == 0) RegularHit();
                  else*/
        GlancingBlow();
      } else
        Blocked();
    }
  }

  private void CriticalHit()
  {
    float damageTotal = 5;
    /*      if (armour > 0) armour--;
          else*/
    damageTotal += 5;
    health -= damageTotal;
    Debug.Log("Critical hit! " + name + " takes " + damageTotal + " damage! " + health + " remaining.");
  }

  private void RegularHit()
  {
    health -= 5;
    Debug.Log("Hit! " + name + " takes 5 damage. " + health + " remaining.");
  }

  private void GlancingBlow()
  {
    //      armour--;
    Debug.Log("Glancing hit! " + name + " defense is lowered! " + /*armour +*/ " remaining.");
  }

  private void Blocked()
  {
    Debug.Log("Blocked! " + name + " takes no damage!");
  }
}
