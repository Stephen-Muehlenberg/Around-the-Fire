using System.Collections;
using System;

/// <summary>
/// An action a hero can perform. Each instance must be added to the <see cref="ActionManager"/>
/// to be usable in-game.
/// </summary>
public abstract class HeroAction
{
  public enum Availability
  {
    AVAILABLE, 
    NOT_ENOUGH_WOOD, 
    NOT_ENOUGH_SUPPLIES,
    NEEDS_A_FIRE,
    HIDDEN
  }

  public abstract string title { get; }
  /// <summary>Message displayed while performing action, e.g. title "Cook" becomes "Cooking".</summary>
  public abstract string titlePresentProgressive { get; }
  public abstract string description { get; }
  public abstract PortraitZone location { get; }
  public virtual int hours => 1; // TODO This might need to be a method, if e.g. a hero's competency can lower the time an action takes.
  /// <summary>
  /// Should this Action end before others (negative), after (positive), or in no particular order (0).
  /// </summary>
  public virtual int completionOrder => 0;

  // TODO Handle logic around whether an action requires a partner (e.g. talk) or
  // target (e.g. heal).

  /// <summary>
  /// Is this Action is selectable by the Hero in the current context?
  /// If not, what's the reason. "Subjective" things look Mood and Rest shouldn't affect
  /// this - this is just about strict limitations, e.g. not enough firewood for a fire.
  /// </summary>
  public virtual Availability AvailableFor(Hero hero, GameState context) => Availability.AVAILABLE;

  /// <summary>
  /// Calculate how much the given <paramref name="hero"/> wants to assign
  /// themselves this Action, from 0 (min) to 1 (max).
  /// </summary>
  public virtual float GetAutoAssignWeight(Hero hero, GameState context) => 0f;

  /// <summary>
  /// True if the <paramref name="hero"/> will allow themselves to be assigned to this
  /// Action.
  /// </summary>
  public virtual bool AcceptedBy(Hero hero, GameState context) => true;

  public virtual string GetAssignmentAnnouncement(Hero hero, GameState context)
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

  public virtual string GetCompletionAnnouncement(Hero hero, GameState context)
  {
    return new string[] {
      "Finished.",
      "All done.",
      "Done here.",
      "Job's done.",
      "What's next?",
    }.Random();
  }

  /// <summary>
  /// Asynchronously performs this action, modifying the <paramref name="currentState"/>.
  /// The <paramref name="previousState"/> is provided so that all actions are performed
  /// based on the starting state, and aren't influenced by each other's results.
  /// Invokes <paramref name="callback"/> when finished.
  /// </summary>
  public abstract IEnumerator Process(Hero hero, GameState previousState, GameState currentState, Action callback);

  /// <summary>
  /// Calculates the auto-assign weight for a standard Action which just
  /// increases some of the Hero's basic stats.
  /// </summary>
  protected float StandardAutoAssignWeight(Hero hero, int health = 0, int hunger = 0, int rest = 0, int mood = 0)
  {
    float healthIncrease = 0;
    float hungerIncrease = 0;
    float restIncrease = 0;
    float moodIncrease = 0;

    // Note: the '0.01f's are to prevent divide by zero errors.
    if (health > 0) healthIncrease = ((hero.health + health) / (hero.health > 0 ? hero.health : 0.01f)) - 1;
    else if (health < 0) healthIncrease = 1 - (hero.health / (hero.health - health > 0 ? hero.health - health : 0.01f));
    if (hunger > 0) hungerIncrease = ((hero.hunger + hunger) / (hero.hunger > 0 ? hero.hunger : 0.01f)) - 1;
    else if (hunger < 0) hungerIncrease = 1 - (hero.hunger / (hero.hunger - hunger > 0 ? hero.hunger - hunger : 0.01f));
    if (rest > 0) restIncrease = ((hero.rest + rest) / (hero.rest > 0 ? hero.rest : 0.01f)) - 1;
    else if (rest < 0) restIncrease = 1 - (hero.rest / (hero.rest - rest > 0 ? hero.rest - rest : 0.01f));
    if (mood > 0) moodIncrease = ((hero.mood + mood) / (hero.mood > 0 ? hero.mood : 0.01f)) - 1;
    else if (mood < 0) moodIncrease = 1 - (hero.mood / (hero.mood - mood > 0 ? hero.mood - mood : 0.01f));
    
    return healthIncrease * 1.2f // Health more important than other stats.
      + hungerIncrease
      + restIncrease
      + moodIncrease * 0.9f; // Mood less important than other stats.
  }

  /// <summary>
  /// Modifies the <paramref name="hero"/>'s stats based on supplied params.
  /// Normal params will show a popup, hidden ones will not.
  /// </summary>
  protected void AdjustStats(Hero hero,
    int? health = null, int? hiddenHealth = null,
    int? hunger = null, int? hiddenHunger = null,
    int? mood = null, int? hiddenMood = null,
    int? rest = null, int? hiddenRest = null)
  {
    int popupOffset = 0;

    if (health != null) {
      hero.health += health.Value;
      StatPopup.Show(hero.portrait, Hero.Stat.HEALTH, health.Value, popupOffset++);
    }
    if (hiddenHealth != null) hero.health += hiddenHealth.Value;

    if (hunger != null) {
      hero.hunger += hunger.Value;
      StatPopup.Show(hero.portrait, Hero.Stat.HUNGER, hunger.Value, popupOffset++);
    }
    if (hiddenHunger != null) hero.hunger += hiddenHunger.Value;

    if (mood != null) {
      hero.mood += mood.Value;
      StatPopup.Show(hero.portrait, Hero.Stat.MORALE, mood.Value, popupOffset++);
    }
    if (hiddenMood != null) hero.mood += hiddenMood.Value;

    if (rest != null) {
      hero.rest += rest.Value;
      StatPopup.Show(hero.portrait, Hero.Stat.REST, rest.Value, popupOffset++);
    }
    if (hiddenRest != null) hero.rest += hiddenRest.Value;
  }

  public Option ToOption()
  {
    return new Option()
    {
      title = this.title,
      // TODO icon = ???
      hoverDescription = this.description,
      // TODO unavailable = ???
      // TODO text overlay = ???
      // TODO bottom text = ???
      reference = this
    };
  }
}
