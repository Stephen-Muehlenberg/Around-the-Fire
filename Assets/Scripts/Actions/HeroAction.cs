using System.Collections;
using System;

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
  /// <summary>
  /// Should this Action end before others (negative), after (positive), or in no particular order (0).
  /// </summary>
  public virtual int completionOrder => 0;

  // TODO Handle logic around whether an action requires a partner (e.g. talk) or
  // target (e.g. heal).

  /// <summary>
  /// True if this Action is selectable by the Hero in the current context.
  /// </summary>
  public virtual bool AvailableFor(Hero hero, CampState context) => true;

  /// <summary>
  /// True if the <paramref name="hero"/> will allow themselves to be assigned to this
  /// Action.
  /// </summary>
  public virtual bool AcceptedBy(Hero hero, CampState context) => true;

  public virtual string GetAssignmentAnnouncement(Hero hero, CampController camp)
  {
    return new string[] {
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
    }.Random();
  }

  public virtual string GetCompletionAnnouncement(Hero hero, CampState context)
  {
    return new string[] {
      "Finished.",
      "All done.",
      "Done here.",
      "Job's done.",
      "What's next?",
    }.Random();
  }

  public virtual IEnumerator Process(Hero hero, CampState previousState, CampState currentState, Action callback)
  {
    callback.Invoke();
    return null;
  }

  protected void RaiseStatsAndShowPopups(Hero hero, params (Hero.Stat, int)[] statDeltas)
  {
    Hero.Stat stat;
    int amount;
    for (int i = 0; i < statDeltas.Length; i++)
    {
      (stat, amount) = statDeltas[i];
      switch (stat)
      {
        case Hero.Stat.HEALTH: hero.health += amount; break;
        case Hero.Stat.HUNGER: hero.hunger += amount; break;
        case Hero.Stat.MORALE: hero.mood += amount; break;
        case Hero.Stat.REST: hero.rest += amount; break;
      }
      StatPopup.Show(hero.portrait, stat, amount, statDeltas.Length - i - 1);
    }
  }
}
