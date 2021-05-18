using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
#if UNITY_EDITOR
namespace Assets.Scripts.SpliteImage
{
    public class PixelEntity
    {
        private Vector2Int position;
        private Color pixelColor;
        private int index;
        public PixelEntity(Vector2Int _coor, Color color, int _index)
        {
            position = _coor;
            pixelColor = color;
            index = _index;
        }

        public Vector2Int Position { get => position; }
        public Color PixelColor { get => pixelColor; }
        public int Index { get => index;}
    }
    public class SpliteImage : MonoBehaviour
    {
        //[SerializeField] private Image texture;
        //[SerializeField] private Image mainTexture;
        [SerializeField] private int spriteToParse = 1;

        public static Action EndParse;
        private int PIXEL_COUNT = 0;

        [SerializeField] private Dictionary<Color, List<PixelEntity>> colorList = new Dictionary<Color, List<PixelEntity>>(3145728);

        private Dictionary<Vector2Int, Color> pixelMatrix = new Dictionary<Vector2Int, Color>(3145728);
        private List<PixelEntity> clearPixels = new List<PixelEntity>();
        private Dictionary<Color, Vector2Int> colorType = new Dictionary<Color, Vector2Int>();

        private List<Vector2Int> directionsList = new List<Vector2Int>()
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
        };

        private void Resetup()
        {
            PIXEL_COUNT = 0;
            colorList.Clear();
            pixelMatrix.Clear();
            clearPixels.Clear();
            colorType.Clear();
        }

        [Button("Start")]
        private void Starto()
        {
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start();
            SaveLoad.Load();
            int count = 0;
            foreach(var mainSprite in SaveLoad.mainSprites)
            {
                var name = $"{mainSprite.name}_colored";
                var coloredSprite = SaveLoad.coloredSprites.Find(x => x.name == name);
                if (coloredSprite == null) continue;
                count++;
                SplitedImage(mainSprite, coloredSprite);
                SaveLoad.SaveCO();
                if (count == spriteToParse) break;
            }
            myStopwatch.Stop();
            Debug.Log($"Milliseconds for split {SaveLoad.mainSprites.Count} images is  {myStopwatch.ElapsedMilliseconds}" +
                $"\n~ {myStopwatch.ElapsedMilliseconds/ count} for one");

            Export();

            //EndParse?.Invoke();
        }

        private void SplitedImage(Sprite main, Sprite colored)
        {
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();

            
            int count = 0;
            //var text = texture.sprite.texture;
            //var text = DuplicateTexture(texture.sprite.texture);
            var coloredTexture = DuplicateTexture(colored.texture);
            //var mainTextPixels = DuplicateTexture(mainTexture.sprite.texture).GetPixels();
            var mainTextPixels = DuplicateTexture(main.texture).GetPixels();

            Color[] pixels = coloredTexture.GetPixels();

            myStopwatch.Start();
            for (int i = 0; i < pixels.Length; i++)
            {
                Vector2Int pixelPosition = i.GetRowCol(coloredTexture.width);
                if (pixels[i].a != 0)
                {
                    var color = pixels[i];
                    pixelMatrix.Add(pixelPosition, color);
                    if (!colorList.ContainsKey(color))
                    {
                        colorList.Add(color, new List<PixelEntity>());
                        colorList[color].Add(new PixelEntity(pixelPosition, color, i));
                    }
                    else
                    {
                        colorList[color].Add(new PixelEntity(pixelPosition, color,  i));
                    }
                    PIXEL_COUNT++;
                }
                else
                {
                    pixels[i] = Color.clear;
                    clearPixels.Add(new PixelEntity(pixelPosition, pixels[i], i));
                    count++;
                }
                //pixelMatrix.Add(pixelPosition, pixels[i]);
            }

            //pixelMatrix = pixels.Select((v, i) => new { index = i, color = v }).Where(x => x.color.a != 0).ToDictionary(k => k.index.GetRowCol(text.width), o => o.color);
            Debug.Log($"Add to dict Time {myStopwatch.ElapsedMilliseconds}");
            myStopwatch.Restart();

            var colorListOrder = colorList.OrderBy(x => x.Value.Count).ToDictionary(k => k.Key, v => v.Value);

            Debug.Log($"Sort Time {myStopwatch.ElapsedMilliseconds}");

            var colorCount = FindColorTypeCount(colorListOrder, PIXEL_COUNT);
            myStopwatch.Restart();
            
            //Debug.Log($"colorCount {colorCount}");

            //SortColorsInDict(ref colorListOrder, colorCount);

            SortColorsInDict_VersionTwo(ref colorListOrder, colorCount);

            Debug.Log($"SortColorsInDict_VersionTwo Time {myStopwatch.ElapsedMilliseconds}");
            myStopwatch.Restart();

            pixels = SetNewPixels(colorListOrder);
            Debug.Log($" SetNewPixels Time {myStopwatch.ElapsedMilliseconds}");
            myStopwatch.Restart();

            Dictionary<Color, List<Color>> neiboursColor = new Dictionary<Color, List<Color>>();

            foreach (var colorT in colorType)
            {
                var newPixels = new Color[pixels.Length];
                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i] != colorT.Key)
                    {
                        newPixels[i] = Color.clear;
                    }
                    else
                    {
                        newPixels[i] = colorT.Key;
                    }
                }

                neiboursColor.Add(colorT.Key, FindNeibors(newPixels, coloredTexture.width));
            }

            foreach (var colorT in colorType)
            {
                var newPixels = new Color[pixels.Length];
                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i] != colorT.Key)
                    {
                        newPixels[i] = Color.clear;
                    }
                    else
                    {
                        newPixels[i] = mainTextPixels[i];
                    }
                }

                CreatePart(newPixels, coloredTexture, main.name, colorT.Key, neiboursColor[colorT.Key]);
            }

            Debug.Log($" CreateParts Time {myStopwatch.ElapsedMilliseconds}");
            myStopwatch.Restart();
            //Debug.Log($"colorListOrder count {colorListOrder.Count}");
            foreach (var color in colorListOrder)
            {
                //Debug.Log($"<color="">Color {color.Key} - {color.Value}");
                var ms = $"Color { color.Key} - { color.Value.Count}";
                Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.Key.r * 255f), (byte)(color.Key.g * 255f), (byte)(color.Key.b * 255f), ms));
                //if (color.Value < 10000) colorList.Remove(color.Key);
            }
            
            int co = 0;
            foreach (var item in colorListOrder)
            {
                co += item.Value.Count;
            }
            Debug.Log($"All pixels {pixels.Length}, without alpha - {PIXEL_COUNT} = {co} ? {PIXEL_COUNT == co}");

            //Texture2D newTexture = new Texture2D(text.width, text.height);
            //newTexture.SetPixels(pixels);
            //newTexture.Apply();

            //var outputTexture = new Texture2D(text.width, text.height);

            //ImageUtils.FloodFill(newTexture, outputTexture, Color.white, 2f, pixelsCount, pos.x, pos.y);
            //outputTexture.Apply();
            //var newSprite = Sprite.Create(newTexture, new Rect(texture.transform.position, new Vector2(text.width, text.height)), texture.transform.position);
            //this.gameObject.AddComponent<Image>().sprite = newSprite;

            //texture.sprite = RemoveTransparent(newTexture, out var w, out var h, out var anchX, out var anchY);
            //texture.sprite = newSprite;

            //byte[] itemBGBytes = texture.sprite.texture.EncodeToPNG();
            //File.WriteAllBytes(Application.dataPath + $"/imgTestNew.png", itemBGBytes);
            myStopwatch.Stop();
            Debug.Log($"count {count} Pixel 0 - {pixels[0]} List {colorList.Count} Time {myStopwatch.ElapsedMilliseconds}");
            Resetup();
        }

        private void CreatePart(Color[] pixels, Texture2D texture, string name, Color color, List<Color> neiborsColors)
        {
            Texture2D newTexture = new Texture2D(texture.width, texture.height);
            newTexture.SetPixels(pixels);
            newTexture.Apply();
            newTexture = RemoveTransparent(newTexture, out var w, out var h, out var anchX, out var anchY);
            SaveLoad.AddImage(newTexture, new Vector2((float)anchX, (float)anchY), new Vector2(w, h), name, color, neiborsColors);
            //byte[] itemBGBytes = newTexture.EncodeToPNG();
            //File.WriteAllBytes(Application.dataPath + $"/Test/img_{name}.png", itemBGBytes);
        }

        private Color[] SetNewPixels(Dictionary<Color, List<PixelEntity>> _colorListOrder)
        {
            //_colorListOrder.ToDictionary(k => k.Value.);
            IEnumerable<PixelEntity> pixels = new List<PixelEntity>();
            foreach (var item in _colorListOrder)
            {
                pixels = pixels.Union(item.Value);
                //Debug.Log($"Pixels.Count() {pixels.Count()}");
            }
            pixels = pixels.Union(clearPixels);
            return pixels.OrderBy(x => x.Index).Select(c => c.PixelColor).ToArray();
        }

        private List<Color> FindNeibors(Color[] pixels, int width)
        {
            var rows = pixels.ToChunksRow(width);
            var column = pixels.ToChunksColumn(width);
            List<Vector2Int> coordiante = new List<Vector2Int>();
            for (int j = 0; j < rows.Count; j++)
            {
                var t = rows[j].Select((v, i) => new { index = i, color = v }).Where(x => x.color.a != 0);
                if (t.Any())
                {
                    coordiante.Add(new Vector2Int(j, t.First().index - 1));
                    coordiante.Add(new Vector2Int(j, t.Last().index + 1));
                }
            }
            
            for (int j = 0; j < column.Count; j++)
            {
                var t = column[j].Select((v, i) => new { index = i, color = v }).Where(x => x.color.a != 0);
                if (t.Any())
                {
                    coordiante.Add(new Vector2Int(j, t.First().index - 1));
                    coordiante.Add(new Vector2Int(j, t.Last().index + 1));
                }
            }

            List<Color> colorList = new List<Color>();
            foreach(var item in coordiante)
            {
                if(pixelMatrix.ContainsKey(item))
                {
                    var t = pixelMatrix[item];
                    if (!colorList.Contains(t))
                    {
                        colorList.Add(t);
                    }
                }
            }

            return colorList;
        }

        private void SortColorsInDict(ref Dictionary<Color, List<PixelEntity>> _colorListOrder, int colorCount)
        {
            (Color, double) maxPersent;
            List<Color> colorsToDelete = new List<Color>();
            foreach (var i in _colorListOrder)
            {
                maxPersent = (Color.clear, 1);

                if (_colorListOrder.Where(x => x.Value.Any() == true).Count() == colorCount) break;

                foreach (var j in _colorListOrder)
                {
                    if (!_colorListOrder[j.Key].Any() || i.Key == j.Key) continue;
                    var persent = CheckPercent(i.Key, j.Key);
                    if (maxPersent.Item2 > persent) maxPersent = (j.Key, persent);
                    //Debug.Log($" Color {maxPersent.Item1} maxPersent {maxPersent.Item2}");
                }


                if (maxPersent != (Color.clear, 1))
                {
                    colorsToDelete.Add(i.Key);
                    foreach (var item in i.Value)
                    {
                        //Debug.Log($" Color {maxPersent.Item1} maxPersent {maxPersent.Item2}");
                        _colorListOrder[maxPersent.Item1].Add(new PixelEntity(item.Position, maxPersent.Item1, item.Index));
                    }
                    if (_colorListOrder[i.Key].Any()) _colorListOrder[i.Key].Clear();
                }
            }
            Debug.Log($"colorsToDelete {colorsToDelete}");
            foreach (var color in colorsToDelete)
            {
                _colorListOrder.Remove(color);
            }
        }

        private void SortColorsInDict_VersionTwo(ref Dictionary<Color, List<PixelEntity>> _colorListOrder, int colorCount)
        {
            List<Color> colorsToDelete = new List<Color>();
                foreach (var i in _colorListOrder)
                {

                    if (_colorListOrder.Where(x => x.Value.Any() == true).Count() == colorCount) break;
                    CompareColor(i.Key, i.Value, ref _colorListOrder, ref colorsToDelete);

                }
            

            foreach (var color in colorsToDelete)
            {
                _colorListOrder.Remove(color);
            }
        }

        
        private void CompareColor(Color colorToDelete, List<PixelEntity> pixelsEntity, ref Dictionary<Color, List<PixelEntity>> _colorListOrder, ref List<Color> _colorsToDelete)
        {
            Dictionary<Color, int> colors = new Dictionary<Color, int>();
            foreach (var entity in pixelsEntity)
            {
                var directions = new List<Vector2Int>(directionsList);
                for (int i = 0; i < 4; i++)
                {
                    for(var j = 0; j < directions.Count; j++)
                    {
                        var newDirection = directions[j];
                        var newDir = entity.Position + newDirection;
                        directions[j] += newDirection;
                        if (!pixelMatrix.ContainsKey(newDir)) continue; 
                        var newColor = pixelMatrix[newDir];

                        if (colorType.ContainsKey(newColor))
                        {
                            if (colors.ContainsKey(newColor))
                            {
                                colors[newColor]++;
                            }
                            else
                            {
                                colors.Add(newColor, 1) ;
                            }
                        }
                    }
                }

                var pixelColor = colors.OrderBy(x => x.Value).Last().Key;
                //var st = $"count {colors[pixelColor]} Dictionary<Color, int> colors {colors.Count} - new color {pixelColor}";
                //st.LogColored(pixelColor);
                pixelMatrix[entity.Position] = pixelColor;
                _colorListOrder[pixelColor].Add(new PixelEntity(entity.Position, pixelColor, entity.Index));
                colors.Clear();
            }

            if (_colorListOrder[colorToDelete].Any()) _colorListOrder[colorToDelete].Clear();
            if(!_colorsToDelete.Contains(colorToDelete))
                _colorsToDelete.Add(colorToDelete);
        }

        private int FindColorTypeCount(Dictionary<Color, List<PixelEntity>> _colorListOrder, int pixelCount)
        {
            int count = 0;
            foreach (var item in _colorListOrder)
            {
                var colorsCount = item.Value.Count;
                var t = (colorsCount * 1000) / pixelCount;
                //Debug.Log($"FindColorTypeCount {item.Key} - {t}");
                if (t > 10)
                {
                    //Debug.Log($"ColorType {item.Key}");
                    colorType.Add(item.Key, item.Value[colorsCount/2].Position);
                    count++;
                }
            }
            return count;
        }

        private Color CompareWithMainColors(Color color)
        {
            (Color, double) matchNum = (Color.clear, 1);
            foreach(var colorT in colorType)
            {
                var num = CheckPercent(color, colorT.Key);
                if (num < matchNum.Item2) matchNum = (colorT.Key, num);
            }
            return matchNum.Item1;
        }

        private double CheckPercent(Color one, Color two)
        {
            var green = Mathf.Abs(two.g - one.g);
            var red = Mathf.Abs(two.r - one.r);
            var blue = Mathf.Abs(two.b - one.b);
            var result = (green + red + blue) / 3;
            //Debug.Log($"{one} =? {two} = result {result}");
            return result;
        }

        private Texture2D DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        private Texture2D RemoveTransparent(Texture2D texture2D, out int width, out int height, out double anchorX, out double anchorY)
        {
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            int minX = int.MaxValue;
            int maxX = int.MinValue;

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

            Debug.Log($"Max MaxY:{maxX} MaxX:{maxY}  Min MinY:{minX} MinX:{minY}");

            var max_X = maxY;
            var max_Y = maxX;
            var min_X = minY;
            var min_Y = minX;

            width = max_X - min_X + 1;
            height = max_Y - min_Y + 1;


            //var newOutput = texture2D.GetPixels(min_X, min_Y, max_X - min_X, max_Y - min_Y);
            var newOutput = texture2D.GetPixels(min_X, min_Y, width, height);
            Texture2D newTexture = new Texture2D(width, height);
            newTexture.SetPixels(newOutput);
            newTexture.Apply();

            //anchorX = (max_X - (width) / 2) * 0.7f;
            //anchorX = (texture2D.width - max_X + width / 2f) * 0.7f;
            anchorX = (min_X + (width / 2d)) * 0.7d;
            //anchorY = (max_Y - (height) / 2) * 0.7f;
            //anchorY = (-1) * (texture2D.height - max_Y + (height / 2f)) * 0.7f;
            //anchorY = (texture2D.height - min_Y + (height / 2f)) * 0.7f;
            anchorY = (min_Y + (height / 2d)) * 0.7d;

            Debug.Log($"anchorX {anchorX} anchorY {anchorY} ");
            return newTexture;
        }

        [MenuItem("AssetDatabase/Export")]
        private void Export()
        {
            var exportedPackageAssetList = new List<string>();

            exportedPackageAssetList.Add("Assets/Resources/Images");
            exportedPackageAssetList.Add("Assets/Resources/Levels");

            AssetDatabase.ExportPackage(exportedPackageAssetList.ToArray(), "ParseData.unitypackage",
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        }
    }
}
#endif