
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Leveldata", order = 1)]
public class SO_Data : ScriptableObject
{
    public Categories categoria;
    public int slicedPices;
    public List<ColorsNeibors> neiborsColors = new List<ColorsNeibors>();
    public List<Sprite> sprites = new List<Sprite>();
    public List<Vector2> anchoredPosition = new List<Vector2>();
    public List<Vector2> sizes = new List<Vector2>();

  

}

[System.Serializable]
public class ColorsNeibors
{
    public Color color;
    public List<Color> neibColors = new List<Color>();
}
