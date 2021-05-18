using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public static class ImageUtils
{

	public struct Point {

		public int x;
		public int y;

		public Point(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	public static void FloodFill(Texture2D readTexture, Texture2D writeTexture, Color sourceColor, float tollerance, int x, int y,UnityAction onComplete=null) {
		var targetColor = Color.red;
		var q = new Queue<Point> (readTexture.width * readTexture.height);
		q.Enqueue (new Point (x, y));
		int iterations = 0;

		var width = readTexture.width;
		var height = readTexture.height;
		while (q.Count > 0) {
			var point = q.Dequeue ();
			var x1 = point.x;
			var y1 = point.y;
			if (q.Count > width * height) {
				throw new System.Exception ("The algorithm is probably looping. Queue size: " + q.Count);
			}

			if (writeTexture.GetPixel (x1, y1) == targetColor) {
				continue;
			}

			writeTexture.SetPixel (x1, y1, targetColor);


			var newPoint = new Point (x1 + 1, y1);
			if (CheckValidity (readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
				q.Enqueue (newPoint);

			newPoint = new Point (x1 - 1, y1);
			if (CheckValidity (readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
				q.Enqueue (newPoint);

			newPoint = new Point (x1, y1 + 1);
			if (CheckValidity (readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
				q.Enqueue (newPoint);

			newPoint = new Point (x1, y1 - 1);
			if (CheckValidity (readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
				q.Enqueue (newPoint);

			iterations++;
		}
		onComplete?.Invoke();
	}
    
	//public static void FloodFillFindNeighborsColorsOfPart(Dictionary<Vector2Int, Color> pixels, int width, int height, Color sourceColor, float tollerance, int x, int y,UnityAction onComplete=null) {
	//	var targetColor = Color.clear;
	//	var q = new Queue<Point> (pixels.Count);
	//	q.Enqueue (new Point (x, y));
	//	int iterations = 0;

	//	while (q.Count > 0) {
	//		var point = q.Dequeue ();
	//		var x1 = point.x;
	//		var y1 = point.y;
	//		if (q.Count > width * height) {
	//			throw new System.Exception ("The algorithm is probably looping. Queue size: " + q.Count);
	//		}

	//		if (writeTexture.GetPixel (x1, y1) == targetColor) {
	//			continue;
	//		}

	//		writeTexture.SetPixel (x1, y1, targetColor);


	//		var newPoint = new Point (x1 + 1, y1);
	//		if (CheckValidity (pixels, pixels.width, pixels.height, newPoint, sourceColor, tollerance))
	//			q.Enqueue (newPoint);

	//		newPoint = new Point (x1 - 1, y1);
	//		if (CheckValidity (pixels, pixels.width, pixels.height, newPoint, sourceColor, tollerance))
	//			q.Enqueue (newPoint);

	//		newPoint = new Point (x1, y1 + 1);
	//		if (CheckValidity (pixels, pixels.width, pixels.height, newPoint, sourceColor, tollerance))
	//			q.Enqueue (newPoint);

	//		newPoint = new Point (x1, y1 - 1);
	//		if (CheckValidity (pixels, pixels.width, pixels.height, newPoint, sourceColor, tollerance))
	//			q.Enqueue (newPoint);

	//		iterations++;
	//	}
	//	onComplete?.Invoke();
	//}

    public static void FloodFill(Texture2D readTexture, Texture2D writeTexture, Color sourceColor, float tollerance, int x, int y, int _iterations, UnityAction onComplete = null)
    {
        var targetColor = Color.red;
        var q = new Queue<Point>(readTexture.width * readTexture.height);
        q.Enqueue(new Point(x, y));
        int iterations = _iterations;

        var width = readTexture.width;
        var height = readTexture.height;
        while (q.Count > 0 || iterations == 0)
        {
            var point = q.Dequeue();
            var x1 = point.x;
            var y1 = point.y;
            if (q.Count > width * height)
            {
                throw new System.Exception("The algorithm is probably looping. Queue size: " + q.Count);
            }

            if (writeTexture.GetPixel(x1, y1) == targetColor)
            {
                continue;
            }

            writeTexture.SetPixel(x1, y1, targetColor);


            var newPoint = new Point(x1 + 1, y1);
            if (CheckValidity(readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
                q.Enqueue(newPoint);

            newPoint = new Point(x1 - 1, y1);
            if (CheckValidity(readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
                q.Enqueue(newPoint);

            newPoint = new Point(x1, y1 + 1);
            if (CheckValidity(readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
                q.Enqueue(newPoint);

            newPoint = new Point(x1, y1 - 1);
            if (CheckValidity(readTexture, readTexture.width, readTexture.height, newPoint, sourceColor, tollerance))
                q.Enqueue(newPoint);

            iterations--;
        }
        onComplete?.Invoke();
    }



    static bool CheckValidity(Texture2D texture, int width, int height, Point p, Color sourceColor, float tollerance) {
		if (p.x < 0 || p.x >= width) {
			return false;
		}
		if (p.y < 0 || p.y >= height) {
			return false;
		}

		var color = texture.GetPixel(p.x, p.y);

		var distance = Mathf.Abs (color.r - sourceColor.r) +  Mathf.Abs (color.g - sourceColor.g) +  Mathf.Abs (color.b - sourceColor.b);
		return distance <= tollerance;
	}
	
	
	public static void FloodFillBorder(this Texture2D aTex, int aX, int aY, Color aFillColor, Color aBorderColor)
        {
            int w = aTex.width;
            int h = aTex.height;
            Color[] colors = aTex.GetPixels();
            byte[] checkedPixels = new byte[colors.Length];
            Color refCol = aBorderColor;
            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();
 
                for (int i = current.x; i < w; i++)
                {
                    if (checkedPixels[i + current.y * w] > 0 || colors[i + current.y * w] == refCol)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    checkedPixels[i + current.y * w] = 1;
                    if (current.y + 1 < h)
                    {
                        if (checkedPixels[i + current.y * w + w] == 0 && colors[i + current.y * w + w] != refCol)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        if (checkedPixels[i + current.y * w - w] == 0 && colors[i + current.y * w - w] != refCol)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    if (checkedPixels[i + current.y * w] > 0 || colors[i + current.y * w] == refCol)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    checkedPixels[i + current.y * w] = 1;
                    if (current.y + 1 < h)
                    {
                        if (checkedPixels[i + current.y * w + w] == 0 && colors[i + current.y * w + w] != refCol)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        if (checkedPixels[i + current.y * w - w] == 0 && colors[i + current.y * w - w] != refCol)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
            }
            aTex.SetPixels(colors);
            aTex.Apply();
        }
    
	
	public static void FloodFillArea(this Texture2D aTex, int aX, int aY, Color aFillColor)
        {
            int w = aTex.width;
            int h = aTex.height;
            Color[] colors = aTex.GetPixels();
            Color refCol = colors[aX + aY * w];
            Queue<Point> nodes = new Queue<Point>();
            nodes.Enqueue(new Point(aX, aY));
            while (nodes.Count > 0)
            {
                Point current = nodes.Dequeue();
                for (int i = current.x; i < w; i++)
                {
                    Color C = colors[i + current.y * w];
                    if (C != refCol || C == aFillColor)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        if (C == refCol && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        if (C == refCol && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
                for (int i = current.x - 1; i >= 0; i--)
                {
                    Color C = colors[i + current.y * w];
                    if (C != refCol || C == aFillColor)
                        break;
                    colors[i + current.y * w] = aFillColor;
                    if (current.y + 1 < h)
                    {
                        C = colors[i + current.y * w + w];
                        if (C == refCol && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y + 1));
                    }
                    if (current.y - 1 >= 0)
                    {
                        C = colors[i + current.y * w - w];
                        if (C == refCol && C != aFillColor)
                            nodes.Enqueue(new Point(i, current.y - 1));
                    }
                }
            }
            aTex.SetPixels(colors);
            aTex.Apply();
        }
 
	
}