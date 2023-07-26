using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

/// <summary>
/// Manages combat logic.
/// </summary>
public class Combat : MonoBehaviour, Combat.PortraitCallbacks, Portrait.EventsCallback
{
  private const float actionPointsPerSecond = 0.333f;

  [SerializeField] private EncounterPanel messagePanel;
  [SerializeField] private SelectOptionUI actionList;
  [SerializeField] private CombatZonesUi portraitUi;
  [SerializeField] private CombatInitiativeUi initiativeUi;
  [SerializeField] private ActionPointUi actionPointUi;
  [SerializeField] private Transform heroPortraitParent;
  [SerializeField] private Transform enemyPortraitParent;
  [SerializeField] private GameObject portraitPrefab;

  public List<Combatant> combatants;
  public List<Combatant> heroes;
  public List<Combatant> enemies;

  public Combatant currentCombatant;
  public Combatant selectedCombatant;
  public float actionPoints;
  private Coroutine actionDelayCoroutine;

  private void Start()
  {
    // Create characters.
    heroes = Game.heroes
      .Select(hero => new HeroCombatant(hero) as Combatant)
      .ToList();
    enemies = GetEnemies();

    // Add them all to the master combatant list.
    combatants = new List<Combatant>(heroes.Count + enemies.Count);
    combatants.AddRange(heroes);
    combatants.AddRange(enemies);
    
    // Random initiative order.
    combatants = combatants.Shuffle().ToList();

    // Random battlefield positions.
    combatants.ForEach(it => it.position = Random.value > 0.5f ? 0 : 1);

    // Set some starting action points.
    actionPoints = 1.5f;

    // Create UI portraits for each combatant.
    combatants.ForEach(combatant => {
      var portrait = Instantiate(portraitPrefab, combatant.isHero ? heroPortraitParent : enemyPortraitParent);
      combatant.SetPortrait(portrait.GetComponent<Portrait>(), null);
      if (combatant.isHero)
        combatant.portrait.Initialise((combatant as HeroCombatant).hero, Portrait.Interactions.CLICKABLE, this);
      combatant.portrait.SetAction(null);
      combatant.portrait.ShowName(false);
    });

    // Show combatant portraits.
    portraitUi.Initialise(
      heroes.Where(it => it.position == 0).Select(it => it.combatPortrait).ToList(),
      heroes.Where(it => it.position == 1).Select(it => it.combatPortrait).ToList(),
      enemies.Where(it => it.position == 1).Select(it => it.combatPortrait).ToList(),
      enemies.Where(it => it.position == 0).Select(it => it.combatPortrait).ToList()
    );

    // Set initial / default actions.
    combatants.ForEach(it =>
      it.SetAction(new CombatAction.Attack() { origin = it }
    ));

    // Wait a moment. Bit of a hack: the layout groups will not initialise
    // in the first frame, so wait a frame before we start accessing their position.
    WaitThenStart();
  }

  private List<Combatant> GetEnemies()
  {
    // TODO This is temp placeholder code.
    var skeletonFighter = new EnemyArchetype()
    {
      name = "Skeleton Fighter",
      health = 20,
      block = 60,
      attack = 55,
      defense = 55,
      icon = "skeleton_halberd"
    };
    var skeletonArcher = new EnemyArchetype()
    {
      name = "Skeleton Archer",
      health = 20,
      block = 25,
      attack = 45,
      defense = 45,
      icon = "skeleton_crossbow"
    };
    return new List<Combatant>()
    {
      new EnemyCombatant(skeletonFighter),
      new EnemyCombatant(skeletonFighter),
      new EnemyCombatant(skeletonFighter),
      new EnemyCombatant(skeletonArcher),
      new EnemyCombatant(skeletonArcher),
    };
  }

  private async void WaitThenStart()
  {
    await Task.Delay(100);
    StartTurn();
  }

  private void StartTurn()
  {
    // Update initiative tracker.
    var nextCombatantsPortraits = combatants
      .Take(3)
      .Select(it => it.portrait.sprite)
      .ToList();
    initiativeUi.Show(nextCombatantsPortraits);

    // Show next combatant and action.
    currentCombatant = combatants.First();
    ShowNextAction(currentCombatant, currentCombatant.action);

    actionDelayCoroutine = StartCoroutine(WaitThenPerformAction());
  }

  private void ShowNextAction(Combatant combatant, CombatAction action)
  {
    combatant.portrait.SetAction(action.name);
    combatant.portrait.SetHighlighted(true);
  }

  private IEnumerator WaitThenPerformAction()
  {
    ShowActionName(currentCombatant.action.name);
    HighlightOrigin(currentCombatant);
    List<Combatant> targets = null; // TODO get targets from action
    HighlightTargets(targets);

    float countdownRemaining = 1.5f;
    while (countdownRemaining > 0)
    {
      ShowCountdown(countdownRemaining);
      countdownRemaining -= Time.deltaTime;
      actionPoints += Time.deltaTime * actionPointsPerSecond;
      if (actionPoints > 3) actionPoints = 3;
      actionPointUi.ShowAP(actionPoints);
      yield return null;
    }

    PerformAction();
  }

  private void ShowActionName(string name)
  {
    // TODO
  }

  private void HighlightOrigin(Combatant origin)
  {
    // TODO
  }

  private void HighlightTargets(List<Combatant> targets)
  {
    // TODO
  }

  private void ShowCountdown(float countdownRemaining)
  {
    // TODO
  }

  private async void PerformAction()
  {
    await currentCombatant.action.Resolve(this);

    // Delete any defeated enemies
    for (int i = enemies.Count - 1; i >= 0; i--)
      if (enemies[i].health == 0)
        enemies.RemoveAt(i);
    for (int i = combatants.Count - 1; i >= 0; i--)
      if (combatants[i].isEnemy && combatants[i].health == 0)
      {
        RemoveEnemy(combatants[i]);
        combatants.RemoveAt(i);
      }

    if (CombatCompleted())
      EndCombat();
    else
      EndTurn();
  }

  private void RemoveEnemy(Combatant enemy)
  {
    portraitUi.RemovePortrait(enemy.combatPortrait);
  }

  private void EndTurn()
  {
    if (currentCombatant.isEnemy)
      currentCombatant.portrait.SetAction(null);
    currentCombatant.portrait.SetHighlighted(false);

    combatants.RemoveAt(0);
    combatants.Add(currentCombatant);
    StartTurn();
  }

  private void InteruptAction()
  {

  }

  private bool CombatCompleted()
  {
    return !heroes.Any(it => it.health > 0)
      || !enemies.Any(it => it.health > 0);
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

  public void OnHoverEnter(Combatant combatant)
  {
  }

  public void OnHoverExit(Combatant combatant)
  {
  }

  private List<(CombatAction, ActionButton.Content)> currentActionsAndButtons;

  public void OnClick(Combatant combatant)
  {
    if (combatant.isHero)
    {
      var actions = (combatant as HeroCombatant).GetActions();

      List<Option> actionButtons = actions
        .Select(action => new Option() {
            title = action.name,
            reference = action
          })
        .ToList();

      actionList.Show(
        options: actionButtons,
        onOptionSelectedCallback: (Option o, int i) => {
          var selectedAction = o.reference as CombatAction;
          selectedAction.origin.SetAction(selectedAction);
        },
        dismissOnSelection: true);
    }
    else
    {
      actionList.Dismiss();
    }
  }

  public interface PortraitCallbacks
  {
    public void OnHoverEnter(Combatant combatant);
    public void OnHoverExit(Combatant combatant);
    public void OnClick(Combatant combatant);
  }

  public List<Combatant> GetOpponantsWithinRangeOf(Combatant thisCombatant)
  {
    var opponents = thisCombatant.isHero ? enemies : heroes;
    return opponents.Any(it => it.position == 1)
      ? opponents.Where(it => it.position == 1).ToList()
      : opponents;
  }
}
