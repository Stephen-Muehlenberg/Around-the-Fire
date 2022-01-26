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
      time = 8,
      heroes = new List<Hero>(heroCount),
      firewood = Random.Range(0f, 20f),
      supplies = Random.Range(0f, 20f),
      journey = new JourneyState()
      {
        hoursRequired = 30,
        hoursTravelled = 0,
      },
    };

    for (int i = 0; i < heroCount; i++)
    {
      Party.currentState.heroes.Add(new Hero()
      {
        name = names[i],
        icon = TEMP_heroSprites[i],
        hunger = Random.Range(15, 80),
        rest = Random.Range(15, 90),
        health = Mathf.Clamp(Random.Range(30, 170), 0, 100),
        mood = Random.Range(8, 95),
      });
    }

    initialized = true;
  }
}
