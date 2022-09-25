using UnityEngine;

public class FireEffects : MonoBehaviour
{
  private static FireEffects singleton;

  [SerializeField] private GameObject[] firesBySize;

  private void Awake()
  {
    if (singleton != null) throw new System.Exception("Can't have multiple FireEffect singleton instances.");
    singleton = this;
  }

  public static void SetState(Camp.FireState state)
  {
    for (int i = 0; i < 3; i++)
      singleton.firesBySize[i].SetActive((int) state == i + 1);
  }
}
