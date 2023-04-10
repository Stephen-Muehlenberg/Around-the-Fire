using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Collection of all the <see cref="Encounter"/>s which can be encountered while
/// travelling.
/// </summary>
public static class TravelEncounters
{
  public static List<string> AllQualifiedNames => new() {
    typeof(MuddyRoads).AssemblyQualifiedName,
    typeof(StrangeBerry).AssemblyQualifiedName,
    typeof(StrangeMushroom).AssemblyQualifiedName,
    typeof(OldCampsite).AssemblyQualifiedName,
  };

  public class MuddyRoads : Encounter
  {
    public override void Start()
    {
      int randomAmount = Random.Range(3, 6);
      state.party.heroes.ForEach(hero => hero.rest -= randomAmount);
      Message("Heavy rains have turned the road into a muddy swamp. The party suffers "
        + randomAmount + " exhaustion.")
        .Show();
    }
  }

  public class StrangeBerry : Encounter
  {
    public override void Start()
    {
      var hero = state.party.heroes.Random();
      string messageEffect;
      if (hero.totalSkill <= Random.Range(0, 100))
      {
        int amount = Random.Range(6, 9);
        messageEffect = "[SURVIVAL FAIL!] " + hero.heShe
          + " mistakes it for a common snack and eats it, but within minutes "
          + hero.heShe + " is feeling sick! -" + amount + " health!";
        hero.health -= amount;
      } else
      {
        int amount = Random.Range(2, 3);
        messageEffect = "[SURVIVAL SUCCESS!] " + hero.heShe
          + " identifies it as an edible delicacy! +" + amount + " fresh supplies!";
        state.party.inventory.foodFresh += amount;
      }
      Message(hero.name + " comes across a strange berry. " + messageEffect)
        .Show();
    }
  }

  public class StrangeMushroom : Encounter
  {
    public override void Start()
    {
      Message("You come across a strange mushroom. Who wants to try it?")
        .Option("Ignore it", Ignore)
        .Options(() =>
          state.party.heroes
            .Select((hero) => (hero.name + " (" + hero.hunger + " hunger)", (Action<int>) OnSelectHero))
            .ToList())
        .Show();
    }

    private void Ignore(int _)
    {
      Message("You ignore the mushroom and continue on your way.")
        .Show();
    }

    private void OnSelectHero(int index)
    {
      Hero hero = state.party.heroes[index - 1];
      string effectMessage;
      if (Random.value > 0.5f)
      {
        int amount = Random.Range(3, 6);
        hero.hunger += amount;
        effectMessage = "It's delicious! " + hero.name + " +" + amount + " hunger!";
      } else
      {
        int amount = Random.Range(1, 10);
        hero.health -= amount;
        effectMessage = "It's poisonous! " + hero.name + " -" + amount + " health!";
      }
      Message(hero.name + " eats the mushroom! " + effectMessage)
        .Show();
    }
  }

  public class OldCampsite : Encounter
  {
    public override void Start()
    {
      string contentMessage;
      if (Random.value > .5)
      {
        int resources = Random.Range(1, 5);
        int rations = Random.Range(0, resources);
        int firewood = resources - rations;
        contentMessage = "Whoever camped here left in a hurry - you find "
          + (rations > 0 ? rations + " ration" + (rations > 1 ? "s" : "") : "")
          + (rations > 0 && firewood > 0 ? " and " : "")
          + (firewood > 0 ? firewood + " firewood" : "")
          + ".";
        state.party.inventory.foodCured += rations;
        state.party.inventory.firewood += firewood;
      }
      else
      {
        contentMessage = " You search the area, but find nothing of value.";
      }
      Message("You come across an abandoned campsite. " + contentMessage)
        .Show();
    }
  }

  public class TEMPLATE : Encounter
  {
    public override void Start()
    {
    }
  }
}
