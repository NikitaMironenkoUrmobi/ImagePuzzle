using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoorPicker : MonoBehaviour
{
    [SerializeField] private Image image;
    public static List<Vector2Int> coors = new List<Vector2Int>();

    //[SerializeField] private Transform container;
    //[SerializeField] private GameObject zoneMarker;
    
    private void Start()
    {
        coors?.Clear();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GetCoordinates();
        }

        if(Input.GetMouseButtonUp(1))
        {
            ClearCoor();
        }
    }

    private void GetCoordinates()
    {
        Texture2D tex = image.sprite.texture;
       
        Rect r=image.rectTransform.rect;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform,Input.mousePosition,Camera.main,out localPoint);

        //Instantiate(zoneMarker,  Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity,container);
        int px = Mathf.Clamp (0,(int)(((localPoint.x-r.x)*tex.width)/r.width),tex.width);
        int py = Mathf.Clamp (0,(int)(((localPoint.y-r.y)*tex.height)/r.height),tex.height);
        Color32 col=tex.GetPixel (px,py);
        Debug.Log($"Coordinates {px} {py} color {col}");
        coors.Add(new Vector2Int(px,py));
    }

    private void ClearCoor()
    {
        Debug.Log($"Coordinates clear");
        coors?.Clear();
    }
}
