using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Helpers;
using Celeste.Mod.WarlockHelper.Components;
using MonoMod.Cil;

namespace Celeste.Mod.WarlockHelper.Entities;

[TrackedAs(typeof(TheoCrystal),true)]

public class CustomTheoCrystal :  TheoCrystal
{
    private bool dieNormally;
    public CustomHoldable CHold;
    
    public bool Theo; //Kills the player when dying, must be carried through the level and doesn't spawn when carried in from elsewhere
    
    public CustomTheoCrystal(EntityData data,Vector2 offset) : base(data,offset)
    {
        RemoveTag(Tags.TransitionUpdate);
        Add(CHold = new CustomHoldable());
        CHold.onDie = () =>
        {
            if (Theo)
            {
                dieNormally = true;
            }
            else
            {
                RemoveSelf();
            }
        };
    }
    
    //Hooks

    internal static void Load()
    {
        IL.Celeste.TheoCrystal.Die += theoCrystalDieMod;
        IL.Celeste.Level.EnforceBounds += levelEnforceBoundsMod;
        IL.Celeste.TheoCrystal.Added += theoCrystalAddedMod;
    }
    internal static void Unload()
    {
        IL.Celeste.TheoCrystal.Die -= theoCrystalDieMod;
        IL.Celeste.Level.EnforceBounds += levelEnforceBoundsMod;
        IL.Celeste.TheoCrystal.Added -= theoCrystalAddedMod;
    }

    private static void theoCrystalAddedMod(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.GotoNext(MoveType.After, instr => instr.MatchStfld<TheoCrystal>(nameof(Level)));
        cursor.EmitLdarg0();
        cursor.EmitDelegate(theoCrystalIsEmpty);
        ILLabel ret = cursor.DefineLabel();
        cursor.EmitBrtrue(ret);
        ILLabel noRemove = cursor.DefineLabel();
        cursor.GotoNextBestFit(MoveType.After,i=>i.MatchLdloc1(),i=>i.MatchLdarg0(),i=>i.MatchBeq(out noRemove));
        cursor.EmitLdloc1();
        cursor.EmitDelegate(theoCrystalIsEmpty);
        cursor.EmitBrtrue(noRemove);
        cursor.Index = -1; //you guys wouldn't just put instructions after the ret Right
        cursor.MarkLabel(ret);
    }
    private static void levelEnforceBoundsMod(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.GotoNext(MoveType.After,instr => instr.MatchCallvirt<Tracker>(nameof(Tracker.GetEntity)) && instr.compGeneric([typeof(TheoCrystal)])); //i did this right, right?
        cursor.EmitDelegate(theoCrystalNullEmpty); //obviously this will fail if there's an empty crystal that gets matched before a theo crystal but Who's making a map like that anyways. Ill fix it later if someone really wants it
    }

    private static TheoCrystal theoCrystalNullEmpty(TheoCrystal tc)
    {
        return tc is CustomTheoCrystal { Theo: false } ? null : tc;
    }

    private static bool theoCrystalIsEmpty(TheoCrystal tc)
    {
        return tc is CustomTheoCrystal { Theo: false };
    }
    private static void theoCrystalDieMod(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.EmitLdarg0();
        cursor.EmitDelegate(theoCrystalCustomDie);
        ILLabel returnLabel=cursor.DefineLabel();
        cursor.EmitBrfalse(returnLabel);
        cursor.Index = -1;
        returnLabel.Target = cursor.Next;
    }

    private static bool theoCrystalCustomDie(TheoCrystal tc)
    {
        if (tc is CustomTheoCrystal cc)
        {
            cc.CHold.onDie?.Invoke();
            return cc.dieNormally;
        }
        return true;
    }
}