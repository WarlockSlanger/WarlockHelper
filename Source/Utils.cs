using Celeste.Mod.WarlockHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Reflection;

namespace Celeste.Mod.WarlockHelper
{
    public static class Utils
    {
        internal static void Log(String message, LogLevel? logLevel = null, String specifier = null)
        {
            String tag = WarlockHelperModule.HelperName;
            if (specifier != null)
            {
                tag += $"/{specifier}";
            }
            Logger.Log(logLevel ?? LogLevel.Verbose,tag,message);
        }

        static public Vector2 BumperSnapDir(Vector2 dir, bool snapUp = true, bool sidesOnly = false)
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
