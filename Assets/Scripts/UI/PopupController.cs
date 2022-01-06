using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
  public static Transform canvasTransform;

  private void Awake()
  {
    canvasTransform = this.transform;
  }
}
