using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class EncounterManager
{
  private List<string> travelEncounters;

  /// <summary>
  /// Returns a new instance of a random <see cref="Encounter"/> which can be
  /// encountered during travel.
  /// </summary>
  // TODO Extend this so it excludes recently encountered events, weights events by how
  // often they've occurred, filters out unique events, etc.
  public void RandomTravelEncounter(GameState state, EncounterPanel ui, UnityAction onCompleteCallback)
  {
    if (travelEncounters == null)
      Initialise();

    string encounterName = travelEncounters.Random();
    Type type = Type.GetType(encounterName);
    Encounter encounter = (Encounter) Activator.CreateInstance(type);
    encounter.state = state;
    encounter.ui = ui;
    encounter.onCompleteCallback = onCompleteCallback;
    encounter.Start();
  }

  // TODO Add events here manually.
  private void Initialise()
  {
    travelEncounters = TravelEncounters.AllQualifiedNames;
  }
}
