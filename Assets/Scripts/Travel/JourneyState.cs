public class JourneyState
{
  public static float EXPECTED_KM_PER_DAY = 32;

  // TODO Store start location, end location, maybe total days travelled.
  public float lengthInKilometres;
  public float estimatedDurationInDays => lengthInKilometres / EXPECTED_KM_PER_DAY;
  public float hoursTravelled;
  public float kilometresTravelled;
  public float fractionComplete => kilometresTravelled / lengthInKilometres;
}
