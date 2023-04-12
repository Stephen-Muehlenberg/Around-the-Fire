using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

/// <summary>
/// An in-game event displayed to the user.
/// </summary>
public abstract class Encounter
{
  public GameState state;
  public EncounterPanel ui;
  public UnityAction onCompleteCallback;

  /// <summary>
  /// Displays the event in the provided <paramref name="ui"/>, using the provided <paramref name="state"/>.
  /// Invokes <paramref name="onCompleteCallback"/> when the event is finished.
  /// </summary>
  public abstract void Start();

  protected UiBuilder Message(string text) => new() { encounter = this, text = text };

  protected class UiBuilder
  {
    public Encounter encounter;
    public string text;
    private List<(string, Action<int>)> options = new();

    public UiBuilder Option(string optionText, Action<int> onOptionSelected)
    {
      options.Add((optionText, onOptionSelected));
      return this;
    }

    public UiBuilder Options(Func<List<(string, Action<int>)>> optionGenerator)
    {
      options.AddRange(optionGenerator.Invoke());
      return this;
    }

    // TODO Allow encounters to add an optional function as an argument, which is called when a
    // button is selected, e.g. after use presses "ok".
    public void Show()
    {
      if (options.Count == 0)
        encounter.ui.Show(text, (_) => encounter.onCompleteCallback.Invoke());
      else
        encounter.ui.Show(text, options
          .Select(it => new EncounterPanel.Option() {
            text = it.Item1,
            onSelectedCallback = it.Item2
          })
          .ToArray());
    }
  }
}