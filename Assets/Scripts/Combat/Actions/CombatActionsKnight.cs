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

    public override async Task Resolve(Combat state)
    {
      var opponentsInRange = state.GetOpponantsWithinRangeOf(origin);
      if (opponentsInRange.Count == 0) return;

      var target = opponentsInRange.Random();

      target.block -= target.block;

      // Inflict damage
      target.TakeDamage(Mathf.RoundToInt(target.block));

      // Pause for a moment
      await Task.Delay(1200);
    }
  }
}
