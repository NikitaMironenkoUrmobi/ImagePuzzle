using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tools
{
    public class CorePointBuilder : MonoBehaviour
    {
        [SerializeField] private Image outline;
        public float TOLERANCE = 1;
        
        private static readonly Vector2Int Negative=new Vector2Int(-1, -1);
        private List<Vector2Int> _coordinates;
        
        public List<Vector2Int> GetCoordinates()
        {
            _coordinates=new List<Vector2Int>();
            var grid = outline.sprite.texture;
            var width = grid.width;
            
            Texture2D output=new Texture2D(grid.width,grid.height);
            
            Texture2D copy=new Texture2D(grid.width,grid.height);
            copy.SetPixels(grid.GetPixels());
            copy.Apply();
            
            Vector2Int coordinates=Vector2Int.one;

            bool isWorking = false;
            for (int i=0;i<2;i++)
            {
                var pixels=grid.GetPixels();
                coordinates = FindCorePixel(pixels, width);
                _coordinates.Add(coordinates);
                isWorking = true;
                StartCoroutine(Fill(copy, output, Color.white, TOLERANCE, coordinates.x, coordinates.y, () => !isWorking,
                    () =>
                    {
                        isWorking = false;
                        copy.SetPixels(output.GetPixels());
                        copy.Apply();
                        //LogPixels(copy.GetPixels());
                    }));

            }
            /*while (coordinates != Negative)
            {
             
            }*/

            Debug.Log($"Completed parsing coordinates amount {_coordinates.Count}");
            _coordinates.ForEach(x =>
            {
                Debug.Log(x);
            });
            return _coordinates;
        }

        private IEnumerator Fill(Texture2D copy, Texture2D write, Color sourceColor, float tollerance, int x, int y,Func<bool> predicat,UnityAction onComplete=null)
        {
            ImageUtils.FloodFill(copy,write,sourceColor, TOLERANCE,x,y,onComplete);
            yield return new WaitUntil(predicat);
        }

        private Vector2Int FindCorePixel(Color[] pixels,int width)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if(pixels[i]==Color.white) return new Vector2Int(i % width, i / width);
            }

            Debug.LogError("No white pixels");
            return Negative;
        }

        private void LogPixels(Color[] pixels)
        {
            foreach (var pixel in pixels)
            {
                Debug.Log(pixel);
            }
        }
    }
}
