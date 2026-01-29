using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Core.Platforms;
using System;
using Celeste.Mod.WarlockHelper.Components;

namespace Celeste.Mod.WarlockHelper;

public static class Extensions
{
    public static void ForceDash(this Player player, Vector2? dashdir = null, bool red = false)
    {
        DashDirOverrider ddr = player.Get<DashDirOverrider>();
        ddr.Set(dashdir);
        if (red)
        {
            player.StateMachine.ForceState(5);
        }
        else
        {
            player.StateMachine.ForceState(2);
        }
    }
}