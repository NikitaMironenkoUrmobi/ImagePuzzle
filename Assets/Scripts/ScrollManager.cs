using System;
using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using Koffie.SimpleTasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ScrollSnaps;

public class ScrollManager : MonoBehaviour
{
    [Serializable]
    public class ScrollControl
    {
        [SerializeField] private Transform container;
        [SerializeField] private GameObject scrollPrefab;
        private SimpleScrollSnap _scroll;
        private GameObject _scrollObject;

        public void Enable()
        {
            _scroll.enabled = true;
        }

        public void Disable()
        {
            Destroy(_scrollObject);
        }

        public void SetInteractable(bool value)
        {
            _scroll.swipeGestures = value;
        }

        public void CreateScroll(out Transform content)
        {
            _scrollObject = Instantiate(scrollPrefab, container);
            _scroll = _scrollObject.GetComponent<SimpleScrollSnap>();
            content = _scrollObject.GetComponent<ScrollRect>().content;

        }

        public void Remove(int index)
        {
            _scroll.Remove(index);
        }
    }
    
    [SerializeField] private Splitter splitter;
    [SerializeField] private GameObject scrollPartPrefab;
    private Transform container;
    [SerializeField] private ScrollControl scrollControl;

    private List<ScrollPart> scrollParts=new List<ScrollPart>();
    
  
    
    
    private void OnEnable()
    {
        GameEventDistributor.OnLevelStarted+= Spawn;
        GameEventDistributor.OnLevelEnded+= OnLevelEnded;

        ScrollPart.OnSelected += DisableScrollInteracting;
        Part.OnPartReleased += EnableScrollInteracting;
        
        Splitter.OnClear += Clear;
        Part.OnPartPlaced += OnPlaced;
    }

    private void OnDisable()
    {
        GameEventDistributor.OnLevelStarted-= Spawn;
        GameEventDistributor.OnLevelEnded-= OnLevelEnded;
        
        ScrollPart.OnSelected -= DisableScrollInteracting;
        Part.OnPartReleased -= EnableScrollInteracting;
        
        Splitter.OnClear -= Clear;
        Part.OnPartPlaced -= OnPlaced;
    }


    private void Clear()
    {
        scrollControl?.Disable();
        scrollParts?.Clear();
        if(container==null) return;
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
        
    }
    private void Spawn()
    {
        scrollControl.CreateScroll(out container);
        splitter.Parts.ForEach(x =>
        {
            if (x.id != int.MinValue)
            {
                var part = Instantiate(scrollPartPrefab, container);
                var pScrollPart = part.GetComponent<ScrollPart>();
                pScrollPart.Init(x.id,x.Image.sprite);
                scrollParts.Add(pScrollPart);
            }
        });
        
        scrollControl.Enable();
    }

    private void OnPlaced(int id)
    {
        scrollParts.ForEach(x =>
        {
            if (x.id == id)
            {
                x.Image.color=Color.black;
                scrollControl.Remove(x.gameObject.transform.GetSiblingIndex());
            }
        });
    }

    private void EnableScrollInteracting(int id)
    {
        scrollControl.SetInteractable(true);
    }

    private void DisableScrollInteracting(int id)
    {
        scrollControl.SetInteractable(false);
    }

    private void OnLevelEnded()
    {
        scrollControl.Disable();
    }

}
