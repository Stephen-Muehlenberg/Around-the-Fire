using System;

public class ACA_Rest : HeroAction
{
  public override string title => "Rest";
  public override string titlePresentProgressive => "Resting";
  public override string description => "Take a load off and unwind.";

  public override string GetCompletionAnnouncement(Hero hero, CampController camp)
  {
    return "Done";
  }

  public override void Process(Hero hero, CampController camp, Action callback)
  {
    callback.Invoke();
  }
}
