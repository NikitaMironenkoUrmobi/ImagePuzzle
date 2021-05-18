using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollPart : MonoBehaviour,IPointerDownHandler
{
    public int id;
    
    [SerializeField] private bool isSelected;
    [SerializeField] private bool isPreselected;

    private Vector2 _startPressPosition;
    
    
    private Image image;
    public Image Image
    {
        get
        {
            if (image == null) image=GetComponent<Image>();
            return image;
        }
    }
    public delegate void ScrollPartEvent(int id);
    public static event ScrollPartEvent OnSelected;

    public void Init(int id,Sprite sprite)
    {
        this.id = id;
        Image.sprite = sprite;
    }

    private void Update()
    {
        PreselectedMove();
        OnMouseRelease();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(isPreselected) return;
        _startPressPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isPreselected = true;
    }

    private void OnMouseRelease()
    {
        if(!isPreselected) return;
        if (Input.GetMouseButtonUp(0) )
        {
            isPreselected = false;
            isSelected = false;
            _startPressPosition = default;
        }
    }
    
   
    private void PreselectedMove()
    {
        if (isPreselected)
        {
            if (Tools.Core.Distance.IsMagnitudeYPassed(_startPressPosition,0.7f))
            {
                //Debug.Log("Selected");
                if(!isSelected)isSelected = true;
                OnSelected?.Invoke(id);
            }
        }
    }
}
