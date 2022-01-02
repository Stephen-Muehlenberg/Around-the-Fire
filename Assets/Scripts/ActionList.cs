using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionList : MonoBehaviour
{
  private static ActionList singleton;

  public Transform listParent;
  [SerializeField] private List<ActionButton> buttons;
  private Action<CampAction> actionSelectedCallback;

  private void Awake()
  {
    if (singleton != null) throw new Exception("ActionList singleton already exists.");
    singleton = this;
    gameObject.SetActive(false);
  }

  public static void Show(List<CampAction> actions, Action<CampAction> callback, CampAction selected = null)
  {
    if (actions.Count > singleton.buttons.Count)
      throw new Exception("Too many actions - can't display " + actions.Count + " in " + singleton.buttons.Count + " buttons.");

    ActionButton button;
    for (int i = 0; i < singleton.buttons.Count; i++)
    {
      button = singleton.buttons[i];
      if (actions.Count > i)
      {
        button.gameObject.SetActive(true);
        button.SetAction(actions[i]);
        button.SetSelected(actions[i] == selected);
      } 
      else
        button.gameObject.SetActive(false);
    }
  
    singleton.actionSelectedCallback = callback;
    singleton.gameObject.SetActive(true);
  }

  public static void Hide()
  {
    singleton.gameObject.SetActive(false);
  }

  public void ClickButton(ActionButton button)
  {
    button.SetSelected(true);
    gameObject.SetActive(false);
    actionSelectedCallback?.Invoke(button.action);
  }
}
