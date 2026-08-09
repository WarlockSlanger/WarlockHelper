using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.WarlockHelper
{
    public static class Util
    {
        public static int mod(int p, int q)
        {
            int val = p % q;
            if (val < 0)
            {
                val += q;
            }

            return val;
        }


        public static Vector2 fromAngle(float length, float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * length;
        }
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
        }

        public class Matrix2x2
        {
            public float A, B, C, D, X, Y;

            public Matrix2x2(float a = 1f, float b = 0f, float c = 0f, float d = 1f, float x = 0f, float y = 0f)
            {
                A = a;
                B = b;
                C = c;
                D = d;
                X = x;
                Y = y;
            }

            public Matrix2x2(float[] arr)
            {
                switch (arr.Length)
                {
                    case 6:
                    {
                        A = arr[0];
                        B = arr[1];
                        C = arr[2];
                        D = arr[3];
                        X = arr[4];
                        Y = arr[5];
                        break;
                    }
                    default:
                    {
                        A = 1f;
                        B = 0f;
                        C = 0f;
                        D = 1f;
                        X = 0f;
                        Y = 0f;
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
            else if (num is <= 0.65f and >= -0.55f)
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
        
        public delegate bool TryConverter<in TInput,TOutput>(TInput input, out TOutput output);

        public static bool TryConvertAll<TInput, TOutput>(TInput[] inputs, out TOutput[] outputs, TryConverter<TInput, TOutput> converter)
        {
            if (inputs == null)
            {
                outputs = null;
                return false;
            }
            int size = inputs.Length;
            outputs = new TOutput[size];
            for (int i = 0; i < size; i++)
            {
                if (!converter(inputs[i], out outputs[i]))
                {
                    outputs = null;
                    return false;
                }
            }
            return true;
        }

        internal static string getter(string name)
        {
            return "get_" + name;
        }

        internal static string setter(string name)
        {
            return "set_" + name;
        }
    }
}
