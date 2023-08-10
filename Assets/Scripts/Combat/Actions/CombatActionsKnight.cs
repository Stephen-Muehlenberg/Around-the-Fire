using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CombatActionsKnight
{
  public class Shieldbreaker : CombatAction
  {
    public override string name => "Shieldbreaker";
    private Combatant target;

    public override Targets GetTargets(Combat state)
    {
      var opponentsInRange = state.GetOpponantsWithinRangeOf(origin);
      if (opponentsInRange.Count == 0) return new Targets();

      target = opponentsInRange.Random();

      return new Targets(target);
    }

    public override async Task Resolve(Combat state)
    {
      target.block -= target.block;

      // Inflict damage
      target.TakeDamage(Mathf.RoundToInt(target.block));
    }
  }
}
