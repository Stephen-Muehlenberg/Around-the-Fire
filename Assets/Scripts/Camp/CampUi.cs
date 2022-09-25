using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampUi : MonoBehaviour, Portrait.EventsCallback
{
  public void OnPointerEnterPortrait(Portrait portrait)
  { }

  public void OnPointerExitPortrait(Portrait portrait)
  { }

  public void OnPortraitClick(Portrait portrait)
  { }

  public void OnPortraitDragStart(Portrait portrait, PointerEventData data)
  { }

  public void OnPortraitDrag(Portrait portrait, PointerEventData data)
  { }

  public void OnPortraitDragEnd(Portrait portrait, PointerEventData data)
  { }


  /*
  public void OnPointerEnterPortrait(HeroPortrait portrait)
  {
    if (uiState != UIState.INTERACTIVE) return;
    portrait.Highlight();
    heroStatsPanel.ShowStatsFor(portrait.hero);
  }

  public void OnPointerExitPortrait(HeroPortrait portrait)
  {
    if (uiState != UIState.INTERACTIVE) return;
    portrait.Unhighlight();
    heroStatsPanel.ShowStatsFor(selectedHero);
  }

  public void OnPointerClickPortrait(HeroPortrait portrait)
  {
    if (uiState != UIState.INTERACTIVE) return;
    SelectHero(portrait.hero);
  }

  public void OnPortraitDragStart(HeroPortrait portrait, PointerEventData data)
  {
    if (uiState != UIState.INTERACTIVE) return;

    uiState = UIState.DRAG_IN_PROCESS;
    portrait.SetRaycastTarget(false);
    SelectHero(portrait.hero, false);

    portrait.transform.localPosition += new Vector3(
      data.delta.x,
      data.delta.y,
      0);// / transform.lossyScale.x; // Thanks to the canvas scaler we need to devide pointer delta by canvas scale to match pointer movement.

//    portrait.transform.SetParent(characterCanvas.transform, true);
    portrait.transform.SetAsLastSibling();
  }

  public void OnPotraitDrag(HeroPortrait portrait, PointerEventData data)
  {
    portrait.transform.localPosition += new Vector3(
      data.delta.x,
      data.delta.y,
      0);// / transform.lossyScale.x; // Thanks to the canvas scaler we need to devide pointer delta by canvas scale to match pointer movement.
  }

  public void OnPotraitDragEnd(Portrait portrait, PointerEventData data)
  {
    var results = new List<RaycastResult>();
    raycaster.Raycast(data, results);

    // Get drag destination.
    HeroSubzoneUi newZone = null;
    foreach (RaycastResult result in results)
    {
      newZone = result.gameObject.GetComponentInParent<HeroSubzoneUi>();
      if (newZone != null) break;
    }

    // If destination can accept this, move there.
    if (newZone != null && newZone.CanAccept(portrait))
      portrait.MoveTo(newZone);
//    else
//      portrait.location.CancelMove(portrait);

    portrait.SetRaycastTarget(true);
    uiState = UIState.INTERACTIVE;
  }

  public void OnPortaitCancelPressed(HeroPortrait portrait)
  {
    portrait.hero.SelectAction(null);
 //   if (selectedHero == portrait.hero)
 //     portrait.location.ShowActions(portrait.hero);
  }*/
}
