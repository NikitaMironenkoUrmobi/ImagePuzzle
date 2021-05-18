using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

#if UNITY_EDITOR
public class AddToAddressable
{
    private static string assetPath = Application.dataPath + $"/Resources/Levels/";
    private static string folderPath = Application.dataPath + $"/ABs/";
    private static string spritePath = Application.dataPath + $"/Resources/Images/";
    private static List<SO_Data> dataAsset = new List<SO_Data>();
    //private static AssetImporter dataAssetImport = new AssetImporter();

    private static List<List<Sprite>> sprites = new List<List<Sprite>>();
    private static List<string> directories = new List<string>();

    [MenuItem("Example/Build Asset Bundles Using BuildMap")]
    static void BuildMapABs()
    {
        if (!Directory.Exists(assetPath))
        {
            Debug.LogError($"Cant find directory {assetPath}");
            return;
        }
        else if (!Directory.Exists(spritePath))
        {
            Debug.LogError($"Cant find directory {spritePath}");
            return;
        }

        directories = Directory.GetDirectories(spritePath).ToList();

        dataAsset = Resources.LoadAll<SO_Data>("Levels").ToList();

        for (int i = 0; i < directories.Count; i++)
        {
            var t = directories[i].Split('/');
            Debug.Log($"Images/{t[t.Length - 1]}");
            sprites.Add(new List<Sprite>(Resources.LoadAll<Sprite>($"Images/{t[t.Length - 1]}").ToList()));

        }
        //dataAssetImport = AssetImporter.GetAtPath(assetPath);
        Debug.Log($"dataAsset {dataAsset.Count} sprites {sprites.Count} ");

        // Create the array of bundle build details.
        AssetBundleBuild[] buildMap = new AssetBundleBuild[dataAsset.Count];

        for (int i = 0; i < buildMap.Length; i++)
        {
            buildMap[i].assetBundleName = dataAsset[i].name;
            Debug.Log($"dataAsset[i].name {dataAsset[i].name}");
            var spriteAssets = new string[sprites[i].Count + 1];

            for (int j = 0; j < spriteAssets.Length - 1; j++)
            {
                spriteAssets[j] = AssetDatabase.GetAssetPath(sprites[i][j]);
                AssetImporter.GetAtPath(spriteAssets[j]).SetAssetBundleNameAndVariant(dataAsset[i].name, "");
                Debug.Log($"AssetDatabase.GetAssetPath(sprites[j]) {AssetDatabase.GetAssetPath(sprites[i][j])} ");
            }

            spriteAssets[spriteAssets.Length - 1] = AssetDatabase.GetAssetPath(dataAsset[i]);
            AssetImporter.GetAtPath(spriteAssets[0]).SetAssetBundleNameAndVariant(dataAsset[i].name, "");

            buildMap[i].assetNames = spriteAssets;

            //var path = $"{folderPath}{buildMap[i].assetBundleName}_AssetBundle/";

            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}

            //BuildPipeline.BuildAssetBundles(path, new AssetBundleBuild[] { buildMap[i] }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }

        BuildPipeline.BuildAssetBundles("Assets/ABs", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

    }
    [MenuItem("Example/Build Asset Bundles Using BuildMap2")]
    static void BuildMapABs2()
    {
        if (!Directory.Exists(assetPath))
        {
            Debug.LogError($"Cant find directory {assetPath}");
            return;
        }
        else if (!Directory.Exists(spritePath))
        {
            Debug.LogError($"Cant find directory {spritePath}");
            return;
        }

        directories = Directory.GetDirectories(spritePath).ToList();

        dataAsset = Resources.LoadAll<SO_Data>("Levels").ToList();

        for (int i = 0; i < directories.Count; i++)
        {
            var t = directories[i].Split('/');
            Debug.Log($"Images/{t[t.Length - 1]}");
            sprites.Add(new List<Sprite>(Resources.LoadAll<Sprite>($"Images/{t[t.Length - 1]}").ToList()));

        }
        //dataAssetImport = AssetImporter.GetAtPath(assetPath);
        Debug.Log($"dataAsset {dataAsset.Count} sprites {sprites.Count} ");

        // Create the array of bundle build details.
        //AssetBundleBuild[] buildMap = new AssetBundleBuild[dataAsset.Count];

        for (int i = 0; i < dataAsset.Count; i++)
        {
            var groupName = dataAsset[i].name;
            AddressableHelper.CreateGroup(groupName);
            Debug.Log($"dataAsset[i].name {groupName}");
            var assetCount = sprites[i].Count + 1;
            for (int j = 0; j < assetCount - 1; j++)
            {
                AddressableHelper.CreateAssetEntry(sprites[i][j], groupName);
            }

            AddressableHelper.CreateAssetEntry(dataAsset[i], groupName);
        }


    }
    [MenuItem("Example/Test")]
    public static void Test()
    {
        TextureImporter textureImport = (TextureImporter)TextureImporter.GetAtPath($"Assets/Resources/Images/ANIMALS_1/ANIMALS_1_0.png");
        textureImport.filterMode = FilterMode.Point;
        textureImport.SaveAndReimport();
    }
}
#endif