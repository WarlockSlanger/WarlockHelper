using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.WarlockHelper
{
    internal static class Utils
    {
        public static class Dir8
        {
            public static readonly Vector2 RIGHT = Vector2.UnitX;
            public static readonly Vector2 DOWNRIGHT = Vector2.UnitX.Rotate((float)Math.PI / 4f);
            public static readonly Vector2 DOWN = Vector2.UnitY;
            public static readonly Vector2 DOWNLEFT = Vector2.UnitY.Rotate((float)Math.PI / 4f);
            public static readonly Vector2 LEFT = -Vector2.UnitX;
            public static readonly Vector2 UPLEFT = (-Vector2.UnitX).Rotate((float)Math.PI / 4f);
            public static readonly Vector2 UP = -Vector2.UnitY;
            public static readonly Vector2 UPRIGHT = (-Vector2.UnitY).Rotate((float)Math.PI / 4f);

            public static readonly Vector2[] DIRS = [RIGHT, DOWNRIGHT, DOWN, DOWNLEFT, LEFT, UPLEFT, UP, UPRIGHT];
        }
        public class Matrix2x2
        {
            public float A,B,C,D,X,Y;

            public Matrix2x2(float a=1f, float b=0f, float c=0f, float d=1f,float x=0f, float y=0f)
            {
                A = a; B = b;
                C = c; D = d;
                X = x; Y = y;
            }
            public Matrix2x2(float[] arr)
            {
                switch (arr.Length)
                {
                    case 4:
                    {
                        A = arr[0];
                        B = arr[1];
                        C = arr[2];
                        D = arr[3];
                        break;
                    }
                    case 6:
                    {
                        A = arr[0]; B = arr[1];
                        C = arr[2]; D = arr[3];
                        X = arr[4]; Y = arr[5];
                        break;
                    }
                    default:
                    {
                        Debug.Log($"Matrix should have size 4 or 6, received {string.Join(',',arr)}", LogLevel.Error, "Matrix2x2");
                        A = 1f; B = 0f;
                        C = 0f; D = 1f;
                        X = 0f; Y = 0f;
                        break;
                    }
                }
            }
        }
        public static Vector2 BumperSnapDir(Vector2 dir, bool snapUp = true, bool sidesOnly = false)
        {
            dir = dir.SafeNormalize(-Vector2.UnitY);
            float num = Vector2.Dot(dir, Vector2.UnitY);
            if (snapUp && num <= -0.7f)
            {
                dir.X = 0f;
                dir.Y = -1f;
            }
            else if (num <= 0.65f && num >= -0.55f)
            {
                dir.Y = 0f;
                dir.X = Math.Sign(dir.X);
            }

            if (sidesOnly && dir.X != 0f)
            {
                dir.Y = 0f;
                dir.X = Math.Sign(dir.X);
            }
            return dir;
        }
    }
}
