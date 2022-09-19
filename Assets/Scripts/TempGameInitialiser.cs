using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a party of random heroes on Awake.
/// Used for testing. Should be removed when no longer needed.
/// </summary>
public class TempGameInitialiser : MonoBehaviour
{
  public enum JourneyDestination { NONE, TO_QUEST, TO_TOWN }

  [SerializeField] private List<Sprite> TEMP_heroSprites;
  [SerializeField] private int TEMP_heroCount;
  [SerializeField] private float startTime;
  [SerializeField] private Vector2 healthRange = new Vector2(30, 180);
  [SerializeField] private Vector2 hungerRange = new Vector2(30, 100);
  [SerializeField] private Vector2 moodRange = new Vector2(20, 100);
  [SerializeField] private Vector2 restRange = new Vector2(50, 110);
  [SerializeField] private bool hasDefaultQuest = false;
  [SerializeField] private JourneyDestination journeyDestination = JourneyDestination.NONE;
  private static bool initialized;

  void Awake()
  {
    if (initialized) return;
    TEMP_CreateRandomParty();
  }

  private void TEMP_CreateRandomParty()
  {
    int heroCount = TEMP_heroSprites.Count < TEMP_heroCount
      ? TEMP_heroSprites.Count
      : TEMP_heroCount;
    string[] names = new string[] { "Alice", "Betty", "Clair", "Diana" };
    if (heroCount > names.Length)
      heroCount = names.Length;

    var gameState = new GameState()
    {
      party = new Party()
      {
        heroes = new List<Hero>(heroCount),
        inventory = new Inventory()
        {
          firewood = Random.Range(0f, 20f),
          supplies = Random.Range(0f, 20f),
        },
        quest = hasDefaultQuest ? new Quest()
        {
          title = "Default Quest",
          description = "Example quest description.",
          distanceFromTownKm = 30
        } : null
      },
      world = new World()
      {
        time = new WorldTime()
        {
          hourOfDay = startTime
        }
      }
    };

    for (int i = 0; i < heroCount; i++)
    {
      gameState.party.heroes.Add(new Hero()
      {
        name = names[i],
        icon = TEMP_heroSprites[i],
        health = Mathf.Clamp(Random.Range(healthRange.x, healthRange.y), 0, 100),
        hunger = Mathf.Clamp(Random.Range(hungerRange.x, hungerRange.y), 0, 100),
        mood = Mathf.Clamp(Random.Range(moodRange.x, moodRange.y), 0, 100),
        rest = Mathf.Clamp(Random.Range(restRange.x, restRange.y), 0, 100),
      });
    }

    if (journeyDestination != JourneyDestination.NONE)
    {
      gameState.party.journey = new Journey()
      {
        distanceKm = gameState.party.quest.distanceFromTownKm,
        startTime = gameState.world.time,
      };
    }

    Game.SetState(gameState);

    initialized = true;
  }
}
