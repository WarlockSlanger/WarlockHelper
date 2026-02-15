using Celeste.Mod.WarlockHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.WarlockHelper
{
    public static class Utils
    {
        public struct Matrix2x2
        {
            public float A,B,C,D;

            public Matrix2x2(float a, float b, float c, float d)
            {
                A = a; B = b;
                C = c; D = d;
            }
        }
        public static Vector2 Transform(this Vector2 vector, Matrix2x2 matrix)
        {
            return new Vector2(vector.X*matrix.A + vector.Y*matrix.B, vector.X*matrix.C + vector.Y*matrix.D);
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

        public static Vector2 DashDirSnap(Vector2 dir)
        {
            float num = dir.Angle();
            int num2 = ((num < 0f) ? 1 : 0);
            float num3 = MathF.PI / 8f - (float)num2 * 0.08726646f;
            if (Calc.AbsAngleDiff(num, 0f) < num3)
            {
                return new Vector2(1f, 0f);
            }
            if (Calc.AbsAngleDiff(num, MathF.PI) < num3)
            {
                return new Vector2(-1f, 0f);
            }
            if (Calc.AbsAngleDiff(num, -MathF.PI / 2f) < num3)
            {
                return new Vector2(0f, -1f);
            }
            if (Calc.AbsAngleDiff(num, MathF.PI / 2f) < num3)
            {
                return new Vector2(0f, 1f);
            }
            return new Vector2(Math.Sign(dir.X), Math.Sign(dir.Y)).SafeNormalize();
        }
    }
}
