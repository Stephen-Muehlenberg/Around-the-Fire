using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a party of random heroes on Awake.
/// Used for testing. Should be removed when no longer needed.
/// </summary>
public class TempGameInitialiser : MonoBehaviour
{
  [SerializeField] private List<Sprite> TEMP_heroSprites;
  [SerializeField] private int TEMP_heroCount;
  [SerializeField] private float startTime;
  [SerializeField] private Vector2 healthRange = new Vector2(30, 180);
  [SerializeField] private Vector2 hungerRange = new Vector2(30, 100);
  [SerializeField] private Vector2 moodRange = new Vector2(20, 100);
  [SerializeField] private Vector2 restRange = new Vector2(50, 110);
  [SerializeField] private float journeyDays = 3;
  [SerializeField] private float journeyFractionComplete = 0;
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

    Party.currentState = new PartyState()
    {
      totalTime = startTime,
      heroes = new List<Hero>(heroCount),
      firewood = Random.Range(0f, 20f),
      supplies = Random.Range(0f, 20f),
      journey = new JourneyState()
      {
        lengthInKilometres = journeyDays * JourneyState.EXPECTED_KM_PER_DAY,
        hoursTravelled = 0,
        kilometresTravelled = 0,
      },
    };

    for (int i = 0; i < heroCount; i++)
    {
      Party.currentState.heroes.Add(new Hero()
      {
        name = names[i],
        icon = TEMP_heroSprites[i],
        health = Mathf.Clamp(Random.Range(healthRange.x, healthRange.y), 0, 100),
        hunger = Mathf.Clamp(Random.Range(hungerRange.x, hungerRange.y), 0, 100),
        mood = Mathf.Clamp(Random.Range(moodRange.x, moodRange.y), 0, 100),
        rest = Mathf.Clamp(Random.Range(restRange.x, restRange.y), 0, 100),
      });
    }

    initialized = true;
  }
}
