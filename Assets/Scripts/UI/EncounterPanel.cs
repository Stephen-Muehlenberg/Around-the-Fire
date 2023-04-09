using System;
using UnityEngine;
using UnityEngine.UI;

public class EncounterPanel : MonoBehaviour
{
  public class Option
  {
    public string text;
    /// <summary>The int argument is the index of the option.</summary>
    public Action<int> onSelectedCallback;
  }

  private static readonly Option defaultContinueOption = new()
  {
    text = "Continue"
  };

  [SerializeField] private TMPro.TMP_Text encounterMainMessage;
  [SerializeField] private Button[] optionButtons;
  [SerializeField] private TMPro.TMP_Text[] optionTexts;

  private void Awake()
  {
    Dismiss();
  }

  /// <summary>
  /// Shows the specified message, and up to 6 optional choices the user can select from.
  /// </summary>
  public void Show(string message, params Option[] options)
  {
    gameObject.SetActive(true);
    encounterMainMessage.text = message;
    for (int i = 0; i < /*6*/3; i++)
      InitialiseOptionButton(i, options.Length > i ? options[i] : null);
  }

  /// <summary>
  /// Shows the specified message, and default "continue" message.
  /// </summary>
  public void Show(string message, Action<int> onContinueClicked)
  {
    gameObject.SetActive(true);
    encounterMainMessage.text = message;
    defaultContinueOption.onSelectedCallback = onContinueClicked;
    InitialiseOptionButton(0, defaultContinueOption);
    for (int i = 1; i < /*6*/3; i++)
      InitialiseOptionButton(i, null);
  }

  public void Dismiss()
  {
    gameObject.SetActive(false);
  }

  private void InitialiseOptionButton(int index, Option option)
  {
    optionButtons[index].gameObject.SetActive(option != null);
    if (option == null) return;
    optionButtons[index].onClick.RemoveAllListeners();
    optionButtons[index].onClick.AddListener(() => option.onSelectedCallback.Invoke(index));
    optionTexts[index].text = option.text;
  }
}
