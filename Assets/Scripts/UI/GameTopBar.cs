using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ScrollSnaps;

public class GameTopBar : MonoBehaviour
{
    [SerializeField] private Button toMenu;

    private void OnEnable()
    {
        toMenu.onClick.AddListener(OpenLevelSelector);
       
    }

    private void OnDisable()
    {
        toMenu.onClick.RemoveListener(OpenLevelSelector);
       
    }

    private void OpenLevelSelector()
    {
        //UIWindows.Instance.Open(WindowType.LevelSelector);
    }

}
