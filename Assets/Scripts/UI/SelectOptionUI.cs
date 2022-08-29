using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles logic for displaying a list of options, and choosing
/// one. The actual UI of the list and buttons doesn't matter,
/// so long as it has the requisite display fields.
/// </summary>
public class SelectOptionUI : MonoBehaviour
{
  [SerializeField] private OptionButton[] buttons;
  [SerializeField] private OptionHoverPopupPanel hoverPanel;
  private Action<Option, int> selectionCallback;
  private bool dismissOnSelection;

  private void Start()
  {
    // Hide buttons by default, unless we've already been instrcuted to show options.
    if (selectionCallback == null)
      Dismiss();
  }

  public void Show(List<Option> options, Action<Option, int> onOptionSelectedCallback, bool dismissOnSelection = true)
  {
    if (options.Count > buttons.Length)
      throw new Exception("Not enough buttons in the UI! Required " + options.Count + " but have " + buttons.Length + ".");

    this.selectionCallback = onOptionSelectedCallback;
    this.dismissOnSelection = dismissOnSelection;

    // TODO show buttons' parent panel

    for (int i = 0; i < buttons.Length; i++)
    {
      if (options.Count > i)
      {
        buttons[i].gameObject.SetActive(true);
        buttons[i].Show(options[i], i, this);
      } else
        buttons[i].gameObject.SetActive(false);
    }
  }

  public void Dismiss()
  {
    selectionCallback = null;
    HideButtonPopup();
    // TODO Hide buttons' parent panel.
  }

  public void OnButtonHoverStart(OptionButton button)
  {
    // TODO start timer. If hover stop not called after like .3 seconds, show hover text.
    ShowButtonPopup(button);
  }

  public void OnButtonHoverStop(OptionButton button)
  {
    // TODO hide popup description
    HideButtonPopup();
  }

  public void OnButtonClick(OptionButton button)
  {
    if (button.option.unavailable) return;

    HideButtonPopup();
    // Copy callback temporarily, as calling Dismiss() clears our normal reference.
    var callback = selectionCallback;
    if (dismissOnSelection) Dismiss();
    callback.Invoke(button.option, button.index);
  }

  private void ShowButtonPopup(OptionButton button)
  {
    hoverPanel.gameObject.SetActive(true);
    hoverPanel.ShowFor(button);
  }

  private void HideButtonPopup()
  {
    hoverPanel.gameObject.SetActive(false);
  }
}
