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
        internal static void Log(String message, LogLevel logLevel = LogLevel.Verbose, String specifier = null)
        {
            String tag = WarlockHelperModule.HelperName;
            if (specifier != null)
            {
                tag += $"/{specifier}";
            }
            Logger.Log(logLevel,tag,message);
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
