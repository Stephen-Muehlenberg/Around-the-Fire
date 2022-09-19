using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TownScene : MonoBehaviour
{
  [SerializeField] private Button leaveTownButton;

  private void Start()
  {
    leaveTownButton.interactable = 
      Game.party.quest != null 
      && !Game.party.quest.complete;
  }

  public void OnClick_EnterGuild()
  {
    SceneManager.LoadScene("Guild");
  }

  public void OnClick_LeaveTown()
  {
    if (Game.party.quest == null || Game.party.quest.complete)
    {
      // TODO Show some sort of error "You can't leave town without a destination!"
    }
    else
    {
      Game.party.journey = new Journey()
      {
        townIsDestination = false,
        distanceKm = Game.party.quest.distanceFromTownKm,
        startTime = Game.world.time,
      };
      SceneManager.LoadScene("Travel");
    }
  }
}
