using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GuildScene : MonoBehaviour
{
  [SerializeField] private Button turnInQuestButton;
  [SerializeField] private Button questBoardButton;

  public void Start()
  {
    questBoardButton.interactable = Game.party.quest == null;
    turnInQuestButton.interactable =
      Game.party.quest != null && Game.party.quest.complete;
  }

  public void OnClick_QuestBoard()
  {
    if (Game.party.quest != null) return;

    UnityEngine.Debug.Log("Obtained new Quest!");
    Game.party.quest = new Quest() {
      title = "Example Quest",
      description = "Go to place and then return.",
      distanceFromTownKm = 10
    };

    questBoardButton.interactable = false;
  }

  public void OnClick_CompleteQuest()
  {
    if (Game.party.quest == null || !Game.party.quest.complete)
      return;

    UnityEngine.Debug.Log("Quest Completed!");
    Game.party.inventory.money += Game.party.quest.rewardMoney;
    Game.party.quest = null;

    turnInQuestButton.interactable = false;
    questBoardButton.interactable = true;
  }

  public void OnClick_Leave()
  {
    SceneManager.LoadScene("Town");
  }
}
