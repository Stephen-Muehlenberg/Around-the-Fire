using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatInitiativeUi : MonoBehaviour
{
  [SerializeField] private Image[] portraits;

  public void Show(List<Sprite> characterSprites)
  {
    if (characterSprites.Count != portraits.Length)
      Debug.LogWarning("Attempting to show " + characterSprites.Count + " portraits in " + portraits.Length + " slots!");

    for (int i = 0; i < portraits.Length; i++)
    {
      if (characterSprites.Count <= i)
        portraits[i].gameObject.SetActive(false);
      else
      {
        portraits[i].sprite = characterSprites[i];
        portraits[i].gameObject.SetActive(true);
      }
    }
  }

  public void Hide()
  {
    for (int i = 0; i < portraits.Length; i++)
      portraits[i].gameObject.SetActive(false);
  }
}
