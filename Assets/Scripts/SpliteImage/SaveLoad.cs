using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.SpliteImage
{
    public class SaveLoad
    {

        public static string folder = "";
        private static int index = 0;
        private static Color partColor = Color.clear;

        private static string assetPath = Application.dataPath + $"/Resources/Levels/";
        private static string coloredImagesPath = Application.dataPath + $"/Resources/ColoredImages/";
        private static string mainImagesPath = Application.dataPath + $"/Resources/MainImages/";
        private static SO_Data dataToSave;

        public static List<Sprite> coloredSprites = new List<Sprite>();
        public static List<Sprite> mainSprites = new List<Sprite>();
        private static List<ColorsNeibors> neiborsColors = new List<ColorsNeibors>();
        private static List<Sprite> SPR = new List<Sprite>();
        private static List<Vector2> ANC = new List<Vector2>();
        private static List<Vector2> SIZ = new List<Vector2>();


        private static void SaveImage(Texture2D texture, string name)
        {
            var dirName = Application.dataPath + $"/Resources/Images/{folder}/";
            byte[] itemBGBytes = texture.EncodeToPNG();
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            //Debug.Log(Application.dataPath + $"/Resources/Images/{folder}/img{index}.png");
            File.WriteAllBytes($"{dirName}{name}.png", itemBGBytes);
        }

        public static void Load()
        {
            if(!Directory.Exists(mainImagesPath))
            {
                Debug.LogError($"Cant find directory {mainImagesPath}");
                return;
            }
            else if(!Directory.Exists(coloredImagesPath))
            {
                Debug.LogError($"Cant find directory {coloredImagesPath}");
                return;
            }

            coloredSprites = Resources.LoadAll<Sprite>("ColoredImages").ToList();
            mainSprites = Resources.LoadAll<Sprite>("MainImages").ToList();

            Debug.Log($"coloredSprites {coloredSprites.Count} mainSprites {mainSprites.Count} oo {mainImagesPath}");
        }

        public static void SaveCO()
        {
            CreateInstance(SPR, ANC, SIZ);
            Resetup();
        }

        private static void Resetup()
        {
            folder = "empty";
            partColor = Color.clear;
            index = 0;
            SPR.Clear();
            ANC.Clear();
            SIZ.Clear();
            neiborsColors.Clear();
        }

        public static void CreateInstance(List<Sprite> sprites, List<Vector2> achors, List<Vector2> sizes)
        {

            SO_Data data = ScriptableObject.CreateInstance<SO_Data>();
            data.categoria = (Categories)Enum.Parse( typeof(Categories), folder.Split('_')[0]);
            data.neiborsColors = new List<ColorsNeibors>(neiborsColors);
            data.sprites = new List<Sprite>(sprites);
            data.anchoredPosition = new List<Vector2>(achors);
            data.sizes = new List<Vector2>(sizes);
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(data, "Assets/Resources/Levels/" + $"{folder}.asset");
            AssetDatabase.SaveAssets();
#endif

        }
        public static void AddImage(Texture2D texture, Vector2 anchored, Vector2 size, string name, Color _color, List<Color> _neiborsColors)
        {
            var newName = $"{name}_{index}";
            folder = name;

            SaveImage(texture, newName);
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            TextureImporter textureImport = (TextureImporter)TextureImporter.GetAtPath($"Assets/Resources/Images/{folder}/{newName}.png");
            textureImport.filterMode = FilterMode.Point;
            textureImport.SaveAndReimport();

            var spr = Resources.Load<Sprite>($"Images/{folder}/{newName}");
            neiborsColors.Add(new ColorsNeibors {color = _color, neibColors = _neiborsColors });
            SPR.Add(spr);
            ANC.Add(anchored);
            SIZ.Add(size);
            index++;
        }
    }
}