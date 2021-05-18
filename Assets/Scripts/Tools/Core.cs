using UnityEngine;

namespace Tools
{
    public class Core 
    {
        public class Distance
        {
            /// <summary>
            /// Check is Magnitude to cursor is passed;
            /// </summary>
            /// <param name="pointA"></param>
            /// <param name="magnitude"></param>
            /// <param name="camera"></param>
            /// <returns></returns>
            public static bool IsMagnitudePassed(Vector2 pointA, float magnitude=0.1f,Camera camera=null)
            {
                if(camera==null) camera=Camera.main;
                var cursor =camera.ScreenToWorldPoint(Input.mousePosition);
                var cursor2d = new Vector2(cursor.x, cursor.y);
                if (Vector2.Distance(pointA, cursor2d) > magnitude)
                {
                    return true;
                }

                return false;
            }

            public static bool IsMagnitudeYPassed(Vector2 pointA, float magnitude=0.1f,Camera camera=null)
            {
                if(camera==null) camera=Camera.main;
                var cursor =camera.ScreenToWorldPoint(Input.mousePosition);
                var cursor2d = new Vector2(cursor.x, cursor.y);
                if (Mathf.Abs(pointA.y-cursor2d.y) > magnitude)
                {
                    return true;
                }

                return false;
            }
           
        }
    }
}
