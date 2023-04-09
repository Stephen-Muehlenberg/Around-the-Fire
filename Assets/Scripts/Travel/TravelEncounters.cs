using System;
using System.Linq;

public class TE_MushroomTest : Encounter
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
    if (UnityEngine.Random.value > 0.5f)
    {
      int amount = UnityEngine.Random.Range(3, 6);
      hero.hunger += amount;
      effectMessage = "It's delicious! " + hero.name + " +" + amount + " hunger!";
    } else
    {
      int amount = UnityEngine.Random.Range(1, 10);
      hero.health -= amount;
      effectMessage = "It's poisonous! " + hero.name + " -" + amount + " health!";
    }
    Message(hero.name + " eats the mushroom! " + effectMessage)
      .Show();
  }
}


public class TE_StrangeBerry : Encounter
{
  public override void Start()
  {
    var hero = state.party.heroes.Random();
    string messageEffect;
    if (hero.totalSkill <= UnityEngine.Random.Range(0, 100))
    {
      int amount = UnityEngine.Random.Range(6, 9);
      messageEffect = "[SURVIVAL FAIL!] " + hero.heShe
        + " mistakes it for a common snack and eats it, but within minutes "
        + hero.heShe + " is feeling sick! -" + amount + " health!";
      hero.health -= amount;
    } else
    {
      int amount = UnityEngine.Random.Range(2, 3);
      messageEffect = "[SURVIVAL SUCCESS!] " + hero.heShe
        + " identifies it as an edible delicacy! +" + amount + " fresh supplies!";
      state.party.inventory.suppliesFresh += amount;
    }
    Message(hero.name + " comes across a strange berry. " + messageEffect)
      .Show();
  }
}

