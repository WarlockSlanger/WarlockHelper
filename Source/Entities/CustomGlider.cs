using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Diagnostics;
using System.Reflection;
using Celeste.Mod.Helpers;
using Celeste.Mod.WarlockHelper.Components;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.WarlockHelper.Entities;

public abstract class CustomGlider : Glider 
{
    public CustomHoldable CHold;
    
    public CustomGlider(EntityData data,Vector2 offset) : base(data,offset)
    {
        Add(CHold = new CustomHoldable());
    }
}