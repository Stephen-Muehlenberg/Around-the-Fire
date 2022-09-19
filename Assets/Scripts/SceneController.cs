using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneController : MonoBehaviour
{
  public static SceneController current;

  public Hero selectedHero;

  [SerializeField] private HeroStatsPanel heroStatsPanel;
  [SerializeField] private ActionList actionList;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }

  public void SelectHero(Hero hero, bool showActions = true)
  {
    if (selectedHero != null)
      selectedHero.portrait.Deselect();
    selectedHero = hero;
    hero.portrait.Select();
    heroStatsPanel.ShowStatsFor(hero);
    if (hero.location != null)
    {
      if (hero.action == null && showActions)
        ShowActionsFor(hero, hero.location);
      else
        actionList.Hide();
    }
  }

  public void ShowActionsFor(Hero hero, HeroLocation location)
  {
    var actions = ActionManager.GetCampActionsFor(location);
    var actionButtons = actions
      .Select(it => new ActionButton.Content()
      {
        text = it.title,
        hoverText = it.description,
        state = (int) it.AvailableFor(hero, Game.state)
      })
      .ToList();
    actionList.Show(actionButtons, (i) => SelectAction(hero, actions[i]));
  }

  public void SelectAction(Hero hero, HeroAction action)
  {
    hero.action = action;
    hero.portrait.ShowSelectedAction(action);
//    CampController.OnActionSelected(this, assignedBySelf);
  //  if (hero.action != null && !wasAssignedBySelf)
    //{
//      string message = hero.action.GetAssignmentAnnouncement(hero, Party.currentState);
  //    SpeechBubble.Show(hero.portrait, message, null);
    //}

    bool allHeroesReady = Game.heroes.All(it => it.action != null);
  //  confirmActionsButton.SetActive(allHeroesReady);
  }
}
