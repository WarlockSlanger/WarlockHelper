using Celeste.Mod.WarlockHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.WarlockHelper
{
    public static class Utils
    {
        internal const string wsh = "WarlockHelper";

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
