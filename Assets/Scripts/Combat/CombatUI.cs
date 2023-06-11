using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUI : MonoBehaviour
{
  [SerializeField] private RectTransform[] targetIcons;
  [SerializeField] private RectTransform[] highlightIcons;

  public void Initialise()
  {
    HideAllHighlights();
    HideAllTargets();
  }

  public void ShowTarget(int index, Combatant combatant)
  {
    targetIcons[index].gameObject.SetActive(true);
    targetIcons[index].position = combatant.portrait.transform.position;
  }

  public void HideTarget(int index)
    => targetIcons[index].gameObject.SetActive(false);

  public void HideAllTargets()
    => targetIcons.ForEach(icon => icon.gameObject.SetActive(false));

  public void ShowHighlight(int index, Combatant combatant)
  {
    highlightIcons[index].gameObject.SetActive(true);
    highlightIcons[index].position = combatant.portrait.transform.position + (Vector3.down * 25);
  }

  public void HideHighlight(int index)
    => highlightIcons[index].gameObject.SetActive(false);

  public void HideAllHighlights()
    => highlightIcons.ForEach(icon => icon.gameObject.SetActive(false));
}
