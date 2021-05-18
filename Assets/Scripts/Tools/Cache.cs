using System.Collections;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class Cache : MonoBehaviour
{

    public static string folder = "animal_3";
    private static int index =0;
    
    private static string assetPath = Application.dataPath+$"/Resources/Levels/";
    private static SO_Data dataToSave;
    
    private static List<Sprite> SPR=new List<Sprite>();
    private static List<Vector2> ANC=new List<Vector2>();
    private static List<Vector2> SIZ=new List<Vector2>();
    
    private static void SaveImage(Texture2D texture)
    {
        byte[] itemBGBytes = texture.EncodeToPNG();
        Debug.Log(Application.dataPath+$"/Resources/Images/{folder}/img{index}.png");
        File.WriteAllBytes( Application.dataPath+$"/Resources/Images/{folder}/img{index}.png" , itemBGBytes );
    }

    
    public static void Save()
    {
        CreateInstance(SPR,ANC,SIZ);
    }
    
    public static void CreateInstance(List<Sprite> sprites, List<Vector2> achors, List<Vector2> sizes)
    {
        
        SO_Data data=ScriptableObject.CreateInstance<SO_Data>();
        data.sprites=new List<Sprite>(sprites);
        data.anchoredPosition=new List<Vector2>(achors);
        data.sizes=new List<Vector2>(sizes);
        #if UNITY_EDITOR
        AssetDatabase.CreateAsset (data, "Assets/Resources/Levels/"+$"{folder}.asset");
        AssetDatabase.SaveAssets();
        #endif
     
    }
    public static void AddImage(Sprite sprite, Vector2 anchored, Vector2 size)
    {
        SaveImage(sprite.texture);
        var spr = Resources.Load<Sprite>($"Images/{folder}/img{index}");
        SPR.Add(spr);
        ANC.Add(anchored);
        SIZ.Add(size);
        index++;
    }
    public static void AddImage(Sprite sprite)
    {
        SaveImage(sprite.texture);
        index++;
    }
}
