using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatantAction : MonoBehaviour
{
  public Combatant origin;
  public Combatant target;

  public abstract void Resolve();

  public class Attack : CombatantAction
  {
    public override void Resolve()
    {
      float attackerSkill = origin.attack;
      // +15 in front, +0 in back
      float attackerPositionBonus = 15 - (origin.position * 5);
      float attackerRandomRoll = Random.Range(1, 101);
      float attackerTotal = attackerSkill
        + attackerPositionBonus
        + attackerRandomRoll;
      Debug.Log(origin.name + " attacks! " + attackerTotal + " (skill " + attackerSkill + ", position " + attackerPositionBonus + ", roll " + attackerRandomRoll + ")");

      float defenderSkill = target.defense;
      // +0 in front, -15 in back
      float defenderPositionBonus = target.position * 5;
      float defenderSkillCheckTarget = 50;
      float defenderTotal = defenderSkill
        + defenderPositionBonus
        + defenderSkillCheckTarget;
      Debug.Log(target.name + " defends! " + defenderTotal + " (skill " + defenderSkill + ", position " + defenderPositionBonus + ", +50 for target)");

      target.TakeHit(attackerTotal - defenderTotal);
    }
  }
}
