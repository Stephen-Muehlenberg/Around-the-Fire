using UnityEngine;

public class FireJitter : MonoBehaviour
{
  public new Light light;

  private Vector3 lightOrigin;

  private void Start()
  {
    lightOrigin = light.transform.position;
  }

  void Update()
  {
    light.transform.position = lightOrigin + new Vector3(Mathf.PerlinNoise(Time.time * 3, 0) * 0.03f, Mathf.PerlinNoise(0, Time.time * 3) * 0.03f, 0);
    light.intensity = 3f  + (Mathf.PerlinNoise(Time.time * 3, 5000) * 2);
  }
}
