using UnityEngine;
using UnityEngine.UI;

using State = TravelScene.TravelState;

/// <summary>
/// Handles all UI for <see cref="TravelScene"/>.
/// </summary>
public class TravelUi : MonoBehaviour
{
  [SerializeField] private TMPro.TMP_Text dayTimeText;
  [SerializeField] private TMPro.TMP_Text speedText;
  [SerializeField] private Image haltResumeButton;
  [SerializeField] private GameObject restButton;
  [SerializeField] private GameObject campButton;
  [SerializeField] private Sprite playSprite;
  [SerializeField] private Sprite pauseSprite;
  [SerializeField] private Sprite stopSprite;
  [SerializeField] private TMPro.TMP_Text destinationText;
  [SerializeField] private TMPro.TMP_Text journeyDurationText;
  [SerializeField] private GameObject arriveButton;

  private string speed;

  public void Initialise(
    int day,
    string timeOfDay,
    string destination,
    int? estimatedJourneyLengthDays)
  {
    destinationText.text = "To <b>" + destination + "</b>";
    if (estimatedJourneyLengthDays.HasValue)
      journeyDurationText.text = estimatedJourneyLengthDays + " day journey";
    else
      journeyDurationText.text = "Unknown distance";

    SetState(State.NONE);
    SetTime(day, timeOfDay);
  }

  public void SetState(State state)
  {
    speedText.gameObject.SetActive(state != State.NONE);
    haltResumeButton.sprite = state switch
    {
      State.TRAVELLING => pauseSprite,
      State.PAUSED => playSprite,
      _ => stopSprite
    };
    haltResumeButton.gameObject.SetActive(state != State.ARRIVED && state != State.NONE);
    restButton.SetActive(state == State.PAUSED);
    campButton.SetActive(state == State.PAUSED);

    arriveButton.SetActive(state == State.ARRIVED);

    speedText.text = state switch
    {
      State.NONE => "",
      State.RESTING => "Resting",
      State.ARRIVED => "Arrived at Destination",
      _ => "Travelling <b>" + speed + "</b>",
    };
  }

  public void SetTime(int day, string timeOfDay)
  {
    dayTimeText.text = "<b>" + timeOfDay + "</b> - Day " + day;
  }

  public void SetSpeed(string speed)
  {
    this.speed = speed;
    speedText.text = "Travelling <b>" + speed + "</b>";
  }
}
