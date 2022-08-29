using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionList : MonoBehaviour
{
  [SerializeField] private GameObject buttonPrefab;
  [SerializeField] private List<ActionButton> buttons;
  private Action<int> onClickCallback;

  public void Show(List<ActionButton.Content> content, Action<int> onClickCallback)
  {
    this.onClickCallback = onClickCallback;

    // Instantiate new buttons if we don't have enough.
    if (content.Count > buttons.Count)
      for (int i = content.Count - buttons.Count; i > 0; i--)
        buttons.Add(Instantiate(buttonPrefab).GetComponent<ActionButton>());

    ActionButton button;
    for (int i = 0; i < buttons.Count; i++)
    {
      button = buttons[i];
      if (content.Count > i)
        button.Set(content[i]);
      else
        button.Set(null);
    }
  }

  public void Hide()
  {
    buttons.ForEach(it => it.gameObject.SetActive(false));
  }

  public void ClickButton(ActionButton button)
  {
    gameObject.SetActive(false);
    onClickCallback.Invoke(button.transform.GetSiblingIndex());
  }
}
