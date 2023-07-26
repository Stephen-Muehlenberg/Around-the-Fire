using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Manages combat logic.
/// </summary>
public class CombatOLD : MonoBehaviour
{
  [SerializeField] private EncounterPanel messagePanel;
  [SerializeField] private SelectOptionUI optionList;
  [SerializeField] private CombatUIOLD ui;
  [SerializeField] private Transform heroPortraitParent;
  [SerializeField] private Transform enemyPortraitParent;
  [SerializeField] private GameObject portraitPrefab;

  private List<CombatantOLD> heroes;
  private List<CombatantOLD> enemies;
  private List<CombatantOLD> combatants;
  private List<CombatantActionOLD> upcomingActions;

  public class StatusEffect
  {
    public enum Expiration { Never, AfterTurn }

    public string name;
    public int amount;
    public Expiration expiration;
    public int expirationValue;
  }

  private void Start()
  {
    // Create characters.
    heroes = Game.heroes
      .Select(hero => new HeroCombatantOLD(hero) as CombatantOLD)
      .ToList();
    enemies = GetEnemies();

    // Randomize their physical position.
    heroes.Shuffle().ForEachIndexed((hero, i) => hero.position = i);
    enemies.Shuffle().ForEachIndexed((enemy, i) => enemy.position = i);

    // Add them all to the master combatant list.
    combatants = new List<CombatantOLD>(heroes.Count + enemies.Count);
    combatants.AddRange(heroes);
    combatants.AddRange(enemies);

    // Create UI portraits for each combatant.
    combatants.ForEach(combatant => {
      var portrait = Instantiate(portraitPrefab, combatant.isHero ? heroPortraitParent : enemyPortraitParent);
      combatant.SetPortrait(portrait.GetComponent<Portrait>(), null);
      combatant.portrait.SetAction(null);
    });

    // Shuffle the list (temp random initiative).
    combatants = combatants.Shuffle().ToList();

    ui.Initialise();

    // Wait a moment. Bit of a hack: the layout groups will not initialise
    // in the first frame, so wait a frame before we start accessing their position.
    WaitThenStart();
  }

  private async void WaitThenStart()
  {
    await Task.Delay(100);
    StartTurn();
  }

  private async void StartTurn()
  {
    // Get next 3 actions and show them to the player.
    upcomingActions = new(3);
    string message = "";
    for (int i = 0; i < 3; i++)
    {
      // Get action.
      upcomingActions.Add(combatants[i].ChooseAction(heroes, enemies));

      // Show origin.
      ui.ShowHighlight(i, upcomingActions[i].origin);
      await Task.Delay(250);

      // Show target.
      ui.ShowTarget(i, upcomingActions[i].target);

      string m = upcomingActions[i].origin.name + " targets " + upcomingActions[i].target.name + "!";
      message += m;
      if (i < 2) message += '\n';
      Debug.Log(m);

      await Task.Delay(600);
    }

    var options = new List<Option>()
    {
      new Option() { title = "Fight", hoverDescription = "+5 to all heroes' stats." },
      new Option() { title = "Defend", hoverDescription = "+20 to defense, -20 to attack." },
      new Option() { title = "Focus Attacks", hoverDescription = "All heroes focus on a single target. +10 to attack." },
    };
    optionList.Show(options, OnPlayerCommandSelected);
  }

  private void OnPlayerCommandSelected(Option option, int index)
  {
    switch (option.title) {
      case "Encourage":

        break;
    };

    ResolveActions(upcomingActions, OnActionsCompleted);
  }

  private async void ResolveActions(List<CombatantActionOLD> actions, UnityAction onActionsComplete)
  {
    for (int i = 0; i < actions.Count; i++)
    {
      if (CombatCompleted()) break;
      await ResolveAction(actions[i]);
      await Task.Delay(500);
      ui.HideHighlight(i);
      ui.HideTarget(i);
    }
    onActionsComplete();
  }

  private async Task ResolveAction(CombatantActionOLD action)
  {
    if (action.origin.health <= 0)
    {
      Debug.Log(action.origin.name + " is KO'd. Skipping turn...");
    } else
    {
      if (action.target.health <= 0)
      {
        Debug.Log(action.origin.name + "'s target is already defeated. Skipping turn...");
      } else
      {
        action.Resolve();

        if (action.target.health <= 0)
        {
          Debug.Log(action.target.name + " defeated!");
          combatants.Remove(action.target);
        }
      }
    }

    await Task.Delay(1000);
  }

  private bool CombatCompleted()
  {
    return !combatants.Any(it => it.isHero && it.health > 0)
      || !combatants.Any(it => it.isEnemy && it.health > 0);
  }

  private void OnActionsCompleted()
  {
    // Move previous combatants' initiative to end of initiative order.
    combatants.RemoveRange(0, 3);
    for (int i = 0; i < 3; i++)
      if (upcomingActions[i].origin.health > 0)
        combatants.Add(upcomingActions[i].origin);

    StartTurn();
  }

  private List<CombatantOLD> GetEnemies()
  {
    // TODO This is temp placeholder code.
    var skeletonFighter = new EnemyArchetype()
    {
      name = "Skeleton Fighter",
      attack = 55,
      defense = 55,
      health = 1,
      icon = "skeleton_halberd"
    };
    var skeletonArcher = new EnemyArchetype()
    {
      name = "Skeleton Archer",
      attack = 45,
      defense = 45,
      health = 1,
      icon = "skeleton_crossbow"
    };
    return new List<CombatantOLD>()
    {
      new EnemyCombatantOLD(skeletonFighter),
      new EnemyCombatantOLD(skeletonFighter),
      new EnemyCombatantOLD(skeletonArcher),
      new EnemyCombatantOLD(skeletonArcher),
    };
  }

  private void EndCombat()
  {
    Game.state.journey = new Journey()
    {
      destination = new Location()
      {
        name = "Nice Town",
        isTown = true
      },
      distanceKm = 10,
      startTime = Game.time.Copy()
    };

    string message;
    if (!combatants.Any(it => it.isHero))
      message = "All heroes defeated!";
    else
      message = "All enemies defeated!";

    messagePanel.Show(message, (_)
      => UnityEngine.SceneManagement.SceneManager.LoadScene("Travel"));
  }
}