using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Koffie.SimpleTasks;
using NaughtyAttributes;
using Tools;
using UnityEngine;
using UnityEngine.UI;



public class Splitter : MonoBehaviour
{
  
    
    
    [SerializeField] private Image gridImage;
    [SerializeField] private Image mainImage;
   
    
    [SerializeField] private List<Image> parts;
    [SerializeField] private CorePointBuilder builder;
    private List<Part> _parts;
    public List<Part> Parts
    {
        get
        {
            if (_parts == null)
            {
                _parts = new List<Part>();
                parts.ForEach(x =>
                {
                    _parts.Add(x.GetComponent<Part>());
                });
                return _parts;
            }
            if (_parts.Count <=0)
            {
                parts.ForEach(x =>
                {
                    _parts.Add(x.GetComponent<Part>());
                });
                return _parts;
            }
            return _parts;
        }
    }

    private List<Vector2Int> partsPixelPos=new List<Vector2Int>
    {
        new Vector2Int(566, 480),
        new Vector2Int(812, 386),
        new Vector2Int(310, 700),
        new Vector2Int(665, 718),
        new Vector2Int(1111, 718),
        new Vector2Int(295, 1140),
        new Vector2Int(590, 1140),
        new Vector2Int(950, 990),
        new Vector2Int(445, 1490),
        new Vector2Int(1100, 1285),
    };
    private Color[] mainPixels;


    public SO_Data data;
    public delegate void SplitterEvent();
    public static event SplitterEvent OnClear;

    private void OnEnable()
    {
        Part.OnPartPlaced += CheckEndGame;
    }

    private void OnDisable()
    {
        Part.OnPartPlaced -= CheckEndGame;
    }

    public void Clear()
    {
        _parts?.Clear();
        mainPixels = new Color[0];
        OnClear?.Invoke();
    }

    public void Play()
    {
        //StartGame();
    }


    #region Tools

    public Part GetPartById(int id)
    {
        foreach (var part in Parts)
        {
            if (part.id == id)
            {
                return part;
            }
        }

        return null;
    }
    

    #endregion

    #region Gameloop

    private void CheckEndGame(int id)
    {
        STasks.DoAfterFrames(() =>
        {
            if (IsGameEnded())
            {
                GameEventDistributor.CallEndLevel();
            }
        },1);
        
    }
    private bool IsGameEnded()
    {
        for (int i = 0; i < partsPixelPos.Count; i++)
        {
            var part=parts[i].GetComponent<Part>();
            if (part.id != int.MinValue && !part.IsPlaced)
            {
                return false;
            }
        }
        Debug.Log("<color=red>Game ended</color>");
        return true;
    }

    private void StartGame()
    {
        for (var i = 0; i < data.sprites.Count; i++)
        {
            CreatePart(parts[i],data.sprites[i],data.anchoredPosition[i],data.sizes[i]);
        }
        GameEventDistributor.CallStartLevel();
    }
    [Button()]
    private void Split()
    {
        partsPixelPos = CoorPicker.coors;
        for (var i = 0; i < partsPixelPos.Count; i++)
        {
            CreatePart(partsPixelPos[i].x,partsPixelPos[i].y,parts[i]);
        }
        Cache.Save();
    }

    [Button()]
    private void Test()
    {
        partsPixelPos = builder.GetCoordinates();
    }
    #endregion
    
    #region Image Parser

    private void CreatePart(Image imgDonor, Sprite sprite, Vector2 anchor, Vector2 size)
    {
        imgDonor.gameObject.SetActive(true);
        imgDonor.sprite = sprite;
        imgDonor.rectTransform.anchoredPosition = anchor;
        imgDonor.rectTransform.sizeDelta = size;
        var part=imgDonor.GetComponent<Part>();
        part.Setup();
    }
    [Obsolete]
    private void CreatePart(int x, int y,Image imgDonor)
    {
        mainPixels = mainImage.sprite.texture.GetPixels();
        var grid = gridImage.sprite.texture;
        imgDonor.gameObject.SetActive(true);
        imgDonor.rectTransform.sizeDelta = gridImage.rectTransform.sizeDelta;
        var size=new Vector2(grid.width,grid.height);
        Texture2D output=new Texture2D(grid.width,grid.height);
        ImageUtils.FloodFill(grid,output,Color.white, 2f,x,y);

        var allOutput = output.GetPixels();


        for (int i = 0; i < allOutput.Length; i++)
        {
            if (allOutput[i] != Color.red)
            {
                allOutput[i]=Color.clear;
                continue;
            }
            allOutput[i] = mainPixels[i];
        }
        output.SetPixels(allOutput);
        output.Apply();
        imgDonor.sprite = RemoveTransparent(output, out var w, out var h,out var anchX, out var anchY);
        imgDonor.rectTransform.sizeDelta=new Vector2(w,h);
        imgDonor.rectTransform.anchoredPosition=new Vector2(anchX,anchY);
        
        Cache.AddImage(imgDonor.sprite,imgDonor.rectTransform.anchoredPosition,imgDonor.rectTransform.sizeDelta);
        var part=imgDonor.GetComponent<Part>();
        part.Setup();
        //outputImage.sprite=Sprite.Create(output,new Rect(Vector2.zero, size),new Vector2(0.5f,0.5f) );
      
    }

    private Sprite RemoveTransparent(Texture2D texture2D, out int width, out int height,out float anchorX,out float anchorY)
    {
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        int minX=int.MaxValue;
        int maxX=int.MinValue;
        
        var pixels = texture2D.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] != Color.clear)
            {
                var rowcol = i.GetRowCol(texture2D.width);
                if (minY > rowcol.y) minY = rowcol.y;
                if (maxY < rowcol.y) maxY = rowcol.y;
                
                if (minX > rowcol.x) minX = rowcol.x;
                if (maxX < rowcol.x) maxX = rowcol.x;
            }
        }

        //Debug.Log($"Max MaxY:{maxX} MaxX:{maxY}  Min MinY:{minX} MinX:{minY}");

        var max_X = maxY;
        var max_Y = maxX;
        var min_X = minY;
        var min_Y = minX;
        
        
        
        var newOutput=texture2D.GetPixels(min_X,min_Y,max_X-min_X,max_Y-min_Y);
        Texture2D newTexture= new Texture2D(max_X-min_X,max_Y-min_Y);
        newTexture.SetPixels(newOutput);
        newTexture.Apply();
        width = max_X - min_X;
        height = max_Y - min_Y;
        Debug.Log($"Middle {max_X-(max_X-min_X)/2}({(max_X-(max_X-min_X)/2)*0.7f});{max_Y-(max_Y-min_Y)/2}({(max_Y-(max_Y-min_Y)/2)*0.7f})");
        anchorX = (max_X - (max_X - min_X) / 2) * 0.7f;
        anchorY = (max_Y - (max_Y - min_Y) / 2) * 0.7f;
        return Sprite.Create(newTexture,new Rect(0,0, width,height),new Vector2(0.5f,0.5f));
    }
    #endregion

}

public static class Extension
{
    public static Vector2Int GetRowCol(this int index,int width)
    {
        var row = (int) (index / width);
        var column = index % width;
        return new Vector2Int(row,column);
    }
    public static Vector2Int GetRowColumn(int index,int width)
    {
        var row = (int) (index / width);
        var column = index % width;
        return new Vector2Int(row,column);
    }

    public static void LogColored(this string text, Color color)
    {
        Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), text));
    }

    public static IEnumerable<List<T>> ToChunks<T>(this IEnumerable<T> items, int chunkSize)
    {
        List<T> chunk = new List<T>(chunkSize);
        foreach (var item in items)
        {
            chunk.Add(item);
            if (chunk.Count == chunkSize)
            {
                yield return chunk;
                chunk = new List<T>(chunkSize);
            }
        }
        if (chunk.Any())
            yield return chunk;
    }
    public static List<List<T>> ToChunksColumn<T>(this T[] items, int chunkSize)
    {
        List<List<T>> list = new List<List<T>>();
        List<T> chunk = new List<T>(chunkSize);
        for (int i = 0; i < items.Count(); i+=chunkSize)
        {
            chunk.Add(items[i]);
            if (chunk.Count == chunkSize)
            {
                list.Add(chunk);
                chunk = new List<T>(chunkSize);
            }
        }

        if (chunk.Any())
            list.Add(chunk);

        return list;
    }
    public static List<List<T>> ToChunksRow<T>(this T[] items, int chunkSize)
    {
        List<List<T>> list = new List<List<T>>();
        List<T> chunk = new List<T>(chunkSize);
        for (int i = 0; i < items.Count(); i++)
        {
            chunk.Add(items[i]);
            if (chunk.Count == chunkSize)
            {
                list.Add(chunk);
                chunk = new List<T>(chunkSize);
            }
        }

        if (chunk.Any())
            list.Add(chunk);

        return list;
    }

    public static IEnumerable<List<T>> ToNChunks<T>(this IReadOnlyCollection<T> items, int chunksCount)
    {
        var chunkSize = items.Count / chunksCount;
        return items.ToChunks(chunkSize);
    }
}
