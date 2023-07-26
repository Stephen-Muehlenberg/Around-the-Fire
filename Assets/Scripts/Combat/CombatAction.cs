using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class CombatAction
{
  abstract public string name { get; }
  public Combatant origin;

  public abstract Task Resolve(Combat state);

  public class Stagger : CombatAction
  {
    public override string name => "Stagger";

    public override async Task Resolve(Combat state)
    {
      origin.block = origin.blockMax;
      origin.previousAction = this;
      origin.action = origin.ChooseAction(state);
     
      StatPopup.Show(origin.portrait, "Recovered", color: Color.white);

      // Pause for a moment
      await Task.Delay(1200);
    }
  }

  public class Attack : CombatAction
  {
    public override string name => "Attack";
    public Combatant target;

    public override async Task Resolve(Combat state)
    {
      // Get opponents in reach.
      var opponentsInReach = state.GetOpponantsWithinRangeOf(origin);

      // If there are no opponents in reach, pass turn.
      if (opponentsInReach.Count == 0)
        return;

      // If previously attacked, and previous target is still
      // in range, continue attacking that target.
      if (origin.previousAction != null
        && origin.previousAction is Attack
        && opponentsInReach.Contains((origin.previousAction as Attack).target))
      {
        target = (origin.previousAction as Attack).target;
      } else
        target = opponentsInReach.Random();

      // Hit effect info.
      int minDamage = 10; // Temp placeholder value
      int maxDamage = 20; // Temp placeholder value
      int damage = Random.Range(minDamage, maxDamage);
      (string, Color)? message = null;

      // Attempt to hit.
      float attackRoll = origin.attack + Random.Range(0, 100);
      float criticalHitTheshold = target.defense + 80;
      float normalHitTheshold = target.defense + 20;
      if (attackRoll >= criticalHitTheshold)
      {
        damage += maxDamage;
        message = ("CRIT", Color.red);
      } else if (attackRoll < normalHitTheshold)
      {
        damage = 0;
        message = ("MISS!", Color.gray);
      }

      // Inflict damage
      target.TakeDamage(Mathf.RoundToInt(damage), message);

      // Pause for a moment
      await Task.Delay(1200);
    }
  }
}

