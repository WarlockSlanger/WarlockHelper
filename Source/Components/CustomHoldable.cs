using System;
using System.Reflection;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.WarlockHelper.Components;

public class CustomHoldable() : Component(true,false)
{
    public enum Throw
    {
        Throw,
        Swat,
        Drop
    }
    public Player Holder;
    public float Dir;

    public Func<Vector2> throwSpeed,throwRecoil,pickupSpeed;
    public Action onDie;
    public Func<bool> canDie;

    public bool Held=false;
    public Throw ThrowType;

    public override void Update()
    {
        base.Update();
        if (canDie?.Invoke() == true)
        {
            onDie?.Invoke();
        }
    }

    //hooks
    
    
    private static ILHook pickupCoroutineHook;
    
    internal static void Load()
    {
        IL.Celeste.Player.Throw += Player_throwMod;
        IL.Celeste.Holdable.Pickup += Holdable_pickupMod;
        IL.Celeste.Holdable.Release += Holdable_releaseMod;
        pickupCoroutineHook =
            new ILHook(
                typeof(Player).GetMethod(nameof(Player.PickupCoroutine), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetStateMachineTarget()!, Player_pickupCoroutineMod);
    }
    internal static void Unload()
    {
        IL.Celeste.Player.Throw -= Player_throwMod;
        IL.Celeste.Holdable.Pickup -= Holdable_pickupMod;
        IL.Celeste.Holdable.Release -= Holdable_releaseMod;
        pickupCoroutineHook.Dispose();
        pickupCoroutineHook = null;
    }

    private static void Holdable_pickupMod(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.GotoNext(MoveType.After, instr => instr.MatchCallvirt<Holdable>(Util.setter(nameof(Holdable.Holder))));
        cursor.EmitLdarg0();
        cursor.EmitDelegate(customHoldablePickup);

    }
    private static void customHoldablePickup(Holdable hold)
    {
        if (hold.Entity.Get<CustomHoldable>() is { } ch)
        {
            if (!ch.Held)
            {
                ch.Held = true;
                ch.Holder = hold.Holder;
                hold.Holder.gliderBoostTimer -= 0.16f; //in Vanilla, gliderBoostTimer is checked after the pickup tween ends in PickupCoroutine. here, speed is set Before the animation starts
                if (ch.pickupSpeed != null)
                {
                    hold.Holder.Speed = ch.pickupSpeed();
                }
            }
        }
        
    }
    private static void Holdable_releaseMod(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.Index = -1;
        cursor.EmitLdarg0();
        cursor.EmitLdarg1();
        cursor.EmitDelegate(customHoldableRelease);
    }

    private static void customHoldableRelease(Holdable hold, Vector2 force)
    {
        Entity entity = hold.Entity;
        if (entity.Get<CustomHoldable>() is { } ch)
        {
            if (force.X == 0f)
            {
                ch.ThrowType = Throw.Drop;
            }
            else if (force.Y == 0f)
            {
                ch.ThrowType = Throw.Throw;
            }
            else
            {
                ch.ThrowType = Throw.Swat;
            }

            ch.Dir = Math.Sign(force.X);
            //wait there exists Holdable.GetSpeed() and Holdable.SetSpeed()?? I don't need ISpeed anymore. Cool stuff!
            if (ch.throwSpeed != null)
            {
                hold.SetSpeed(ch.throwSpeed());
            }
            if (ch.throwRecoil != null)
            {
                ch.Holder.Speed = ch.throwRecoil();
            }
            ch.Held = false;
        }
    }
    private static void Player_pickupCoroutineMod (ILContext il)
    {
        ILCursor cursor = new(il);
        
        cursor.GotoNextBestFit(MoveType.AfterLabel,jellyStopTarget);
        cursor.EmitLdloc1(); //i know your tricks now, IEnumerable. "this" is Local 1
        cursor.EmitDelegate(playerGetHasCustomPickupSpeed);
        ILLabel afterStop = cursor.DefineLabel();
        cursor.EmitBrtrue(afterStop);
        
        cursor.GotoNextBestFit(MoveType.After,jellyStopTarget);
        cursor.MarkLabel(afterStop);
        
        cursor.GotoNextBestFit(MoveType.AfterLabel,jellyBoostTarget);
        cursor.EmitLdloc1();
        cursor.EmitDelegate(playerGetHasCustomPickupSpeed);
        ILLabel afterBump = cursor.DefineLabel();
        cursor.EmitBrtrue(afterBump);
        
        cursor.GotoNextBestFit(MoveType.After,jellyBumpEndTarget);
        cursor.MarkLabel(afterBump);
    }
    private static void Player_throwMod(ILContext il)
    {
        ILCursor cursor = new(il);
        
        cursor.GotoNextBestFit(MoveType.Before, playerSpeedTarget);
        ILLabel postSpeed = cursor.DefineLabel();
        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerGetHasCustomThrowRecoil);
        cursor.EmitBrtrue(postSpeed);
        
        cursor.GotoNextBestFit(MoveType.After, playerSpeedEndTarget);
        cursor.MarkLabel(postSpeed);
    }

    private static readonly Func<Instruction, bool>[] playerSpeedTarget =
        [
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdflda<Player>(nameof(Player.Speed))
        ],
        playerSpeedEndTarget =
        [
            instr => instr.MatchConvR4(),
            instr => instr.MatchMul(),
            instr => instr.MatchAdd(),
            instr => instr.MatchStindR4(),
        ],
        throwBranchConditionTarget =
        [
            instr => instr.MatchLdsfld(typeof(Input), nameof(Input.MoveY)),
            instr => instr.MatchLdfld<VirtualIntegerAxis>(nameof(VirtualIntegerAxis.Value)),
            instr => instr.MatchLdcI4(1),
            instr => instr.MatchBneUn(out _)
        ],
        holdingCheckTarget =
        [
            instr => instr.MatchLdarg0(),
            instr => instr.MatchCallvirt<Player>(Util.getter(nameof(Player.Holding))),
            instr => instr.MatchBrfalse(out _)
        ],
        jellyStopTarget =
        [
            instr => instr.MatchLdloc1(),
            instr => instr.MatchLdflda<Player>(nameof(Player.Speed)),
            instr => instr.MatchLdloc1(),
            instr => instr.MatchLdflda<Player>(nameof(Player.Speed)),
            instr => instr.MatchLdfld<Vector2>(nameof(Vector2.Y)),
            instr => instr.MatchLdcR4(0f),
            instr => instr.MatchCall(out _),
            instr => instr.MatchStfld<Vector2>(nameof(Vector2.Y))
        ],
        jellyBoostTarget =
        [
            instr => instr.MatchLdloc1(),
            instr => instr.MatchLdfld<Player>(nameof(Player.gliderBoostTimer)),
            instr => instr.MatchLdcR4(0f),
            instr => instr.MatchBleUn(out _)
        ],
        jellyBumpEndTarget =
        [
            instr => instr.MatchLdcR4(-105f),
            instr => instr.MatchCall(out _),
            instr => instr.MatchStfld<Vector2>(nameof(Vector2.Y)) //bro thinks he would misspell "Y"
        ];
    
    
    private static bool playerGetHasCustomThrowRecoil(Player player)
    {
        return player.Holding.Entity?.Get<CustomHoldable>()?.throwRecoil != null;
    }
    private static bool playerGetHasCustomPickupSpeed(Player player)
    {
        return player.Holding?.Entity.Get<CustomHoldable>()?.pickupSpeed != null;
    }

}