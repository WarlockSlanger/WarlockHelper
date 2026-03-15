using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DataPlayer : Component
{
    public bool forcedDash;
    public DataPlayer() : base(active: true, visible: false)
    {
        forcedDash = false;
    }
}