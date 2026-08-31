using MonoMod.ModInterop;
using System;

namespace Celeste.Mod.WarlockHelper.Imports;

[ModImportName("GravityHelper")]
public static class GravityHelperImports
{
    public static Func<Actor, bool> IsActorInverted;
}