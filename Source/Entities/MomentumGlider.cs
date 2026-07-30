using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.WarlockHelper.Components;
using MonoMod.Cil;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/MomentumGlider")]
public class MomentumGlider : CustomGlider
{
    public bool MomentumDrop, MomentumThrow, MomentumPickup;


    public MomentumGlider(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        MomentumDrop = data.Bool("momentumDrop", false);
        MomentumThrow = data.Bool("momentumThrow", true);
        MomentumPickup = data.Bool("momentumPickup", true);
        CHold.throwSpeed = () =>
        {
            if (CHold.ThrowType==CustomHoldable.Throw.Throw)
            {
                return (MomentumThrow ? CHold.Holder.Speed*0.6f : Vector2.Zero) + new Vector2(CHold.Dir * 100, -40f);
            }
            return MomentumDrop ? CHold.Holder.Speed : Vector2.Zero;
        };
        CHold.pickupSpeed = () =>
        {
            Vector2 playerSpeed=CHold.Holder.GliderBoost(true);
            if (!MomentumPickup)
            {
                return playerSpeed;
            }

            Vector2 gliderSpeed = Speed;
            if (gliderSpeed.Y is > 0f and < 30f)
            {
                gliderSpeed.Y = 0f;
            }

            return playerSpeed + gliderSpeed;
        };
        //Hold.OnPickup = CustomOnPickup;
        //Hold.OnRelease = CustomOnRelease;
    }

    private void CustomOnPickup()
    {
        OnPickup();
    }

    private void CustomOnRelease(Vector2 force)
    {
        OnRelease(force);
        
    }
}