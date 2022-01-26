using UnityEngine;

/// <summary>
/// Adds jitter to light position and intensity.
/// </summary>
public class FireJitter : MonoBehaviour
{
  [SerializeField] private new Light light;
  [SerializeField] private float intensityVariance;

  private Vector3 lightOrigin;
  private float baseIntensity;

  private void Start()
  {
    lightOrigin = light.transform.position;
    baseIntensity = light.intensity;
  }

  void Update()
  {
    light.transform.position = lightOrigin + new Vector3(Mathf.PerlinNoise(Time.time * 3, 0) * 0.03f, Mathf.PerlinNoise(0, Time.time * 3) * 0.03f, 0);
    light.intensity = baseIntensity  + ((Mathf.PerlinNoise(Time.time * 3, 5000) - 0.5f) * 2 * intensityVariance);
  }
}
