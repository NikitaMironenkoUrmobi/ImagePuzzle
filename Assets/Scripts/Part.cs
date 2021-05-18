using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Koffie.SimpleTasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Part : MonoBehaviour
{
    public int id=int.MinValue;
    
    [SerializeField] private bool isSelected;
    [SerializeField] private bool isPlaced;
    public bool IsPlaced => isPlaced;

    [SerializeField]private PartData data;
    private Image image;
    public Image Image
    {
        get
        {
            if (image == null) image = GetComponent<Image>();
            if (!gameObject.activeSelf) isPlaced = false;
            return image;
        }
    }
    public delegate void OnPartEvent(int id);
    public static event OnPartEvent OnPartPlaced;
    public static event OnPartEvent OnPartReleased;

    private void Awake()
    {
        Splitter.OnClear += Clear;
        ScrollPart.OnSelected += Hide;
    }

    private void OnDestroy()
    {
        Splitter.OnClear -= Clear;
        ScrollPart.OnSelected -= Hide;
    }

    public void Clear()
    {
        
        id = int.MinValue;
        Hide();
     

    }
    public void Setup()
    {
        id = gameObject.GetInstanceID();
        data.Collect(Image.rectTransform,id);
        Hide();
    }

    public void FinishPlacing()
    {
        data.Use(Image.rectTransform);
        if (gameObject.activeSelf)
        {
            isPlaced = true;
            isSelected = false;
        }
    }

    public void Release()
    {
        OnPartReleased?.Invoke(id);
        if (IsNear())
        {
            FinishPlacing();
            return;
        }
        Hide();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && gameObject.activeSelf &&  isSelected && !isPlaced)
        {
            Drag();
        }
    }

    void Drag()
    {
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen ));
        transform.position = new Vector3( pos_move.x, pos_move.y,transform.position.z  );
    }
    private bool IsNear()
    {
        if (Vector2.Distance(Image.rectTransform.anchoredPosition,data.AnchoredPosition)<50)
        {
            return true;
        }
        return false;
    }

    public void Appear()
    {
        if (isPlaced || isSelected) return;
        isSelected = true;
        gameObject.transform.position=new Vector3(-1000,-1000);
        gameObject.transform.SetAsLastSibling();
        gameObject.SetActive(true);

    }
    public void Hide()
    { 
        gameObject.SetActive(false);
        if(isSelected) isPlaced = false;
        isSelected = false;
    }

    private void Hide(int catchId)
    {
        if(isPlaced) return;
        if (catchId != id)
        {
            try
            {
                Hide();
            }
            catch
            {
                //ignored
            }
        }
    }
    
    [Serializable]
    private class PartData
    {
        [SerializeField] private Vector2 anchoredPosition;
        public Vector2 AnchoredPosition => anchoredPosition;

        [SerializeField]private Vector2 size;
        private int id;

        public void Collect(RectTransform rect,int id)
        {
            this.id = id;
            anchoredPosition = rect.anchoredPosition;
            size = rect.sizeDelta;
        }

        public void Use(RectTransform rect)
        {
            rect.DOAnchorPos(anchoredPosition,0.2f);
            rect.DOSizeDelta(size,0.2f);
            OnPartPlaced?.Invoke(id);
        }

    }

    
}
