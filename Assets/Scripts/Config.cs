using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
   private void Awake()
   {
      Application.targetFrameRate = 60;
      Input.multiTouchEnabled = false;
   }
}
