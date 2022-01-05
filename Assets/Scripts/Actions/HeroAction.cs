using System;
using UnityEngine;

/// <summary>
/// An action a hero can perform. Each instance must be added to the <see cref="ActionManager"/>
/// to be usable in-game.
/// </summary>
public abstract class HeroAction
{
  public abstract string title { get; }
  /// <summary>Message displayed while performing action, e.g. title "Cook" becomes "Cooking".</summary>
  public abstract string titlePresentProgressive { get; }
  public abstract string description { get; }
  public virtual int hours => 1; // TODO This might need to be a method, if e.g. a hero's competency can lower the time an action takes.

  // TODO Handle logic around whether an action requires a partner (e.g. talk) or
  // target (e.g. heal).

  /// <summary>
  /// True if this Action is selectable by the Hero in the current context.
  /// </summary>
  public virtual bool AvailableFor(Hero hero, CampController camp) => true;

  public virtual string GetAssignmentAnnouncement(Hero hero, CampController camp)
  {
    string[] messages = new string[] {
      "Alright.",
      "Fine.",
      "Got it.",
      "If you say so.",
      "Ok.",
      "Okey dokey.",
      "On it!",
      "Roger.",
      "Sure.",
      "Will do.",
      "With pleasure!",
      "Why not?",
    };
    return messages[UnityEngine.Random.Range(0, messages.Length)];
  }

  public virtual string GetCompletionAnnouncement(Hero hero, CampController camp)
  {
    string[] messages = new string[] {
      "Finished.",
      "All done.",
      "Done here.",
      "Job's done.",
      "What's next?",
    };
    return messages[UnityEngine.Random.Range(0, messages.Length)];
  }

  public virtual void Process(Hero hero, CampController camp, Action callback)
  {
    callback.Invoke();
  }
}
