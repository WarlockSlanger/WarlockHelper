using Celeste.Mod.Entities;
using Celeste.Mod.WarlockHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/MomentumTheoCrystal")]
public class MomentumTheoCrystal : CustomTheoCrystal
{
    public bool MomentumDrop, MomentumThrow, MomentumPickup;
    public MomentumTheoCrystal(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        Theo = data.Bool("hasTheo", false);
        MomentumDrop = data.Bool("momentumDrop", false);
        MomentumThrow = data.Bool("momentumThrow", true);
        MomentumPickup = data.Bool("momentumPickup", true);
        
        Remove(sprite);
        Add(sprite = GFX.SpriteBank.Create(Theo ? "warlockHelper_momentumTheo" : "warlockHelper_momentumCrystal"));
        sprite.Scale.X = -1f; //idk why they did this but sure
        
        CHold.throwSpeed = () =>
        {
            if (CHold.ThrowType==CustomHoldable.Throw.Throw)
            {
                return (MomentumThrow ? CHold.Holder.Speed*0.6f : Vector2.Zero) + new Vector2(CHold.Dir * 200f, -80f);
            }

            if (CHold.ThrowType == CustomHoldable.Throw.Drop)
            {
                return MomentumDrop ? CHold.Holder.Speed : Vector2.Zero;
            }

            return (MomentumDrop ? CHold.Holder.Speed : Vector2.Zero) + new Vector2(CHold.Dir*160f,-50f);
        };
        CHold.pickupSpeed = () =>
        {
            Vector2 playerSpeed=CHold.Holder.Speed;
            if (!MomentumPickup)
            {
                return playerSpeed;
            }
            
            Vector2 theoSpeed = Speed;
            
            if (ChargedSpeed(theoSpeed))
            {
                Audio.Play("event:/game/01_forsaken_city/birdbros_thrust", Position);
            }
            if (theoSpeed.Y is > 0f and < 160f)
            {
                theoSpeed.Y = 0f;
            }

            return playerSpeed + theoSpeed;
        };
    }

    public override void Update()
    {
        base.Update();
        sprite.Play(ChargedSpeed(ChargeSpeed) ? "charged" : "idle");
        if (Scene.OnInterval(0.05f))
        {
            float num = Calc.Random.NextAngle();
            SceneAs<Level>().Particles.Emit(ParticleTypes.SparkyDust, 1, Center + new Vector2(0f,-5f) + Calc.AngleToVector(num, 8f), Vector2.One * 2f, num);
        }
    }

    private Vector2 ChargeSpeed => Hold.IsHeld ? (MomentumThrow ? Hold.Holder.Speed : Vector2.Zero) : (MomentumPickup ? Speed : Vector2.Zero);

    private bool ChargedSpeed(Vector2 speed)
    {
        speed.X *= 1.4f;
        return speed.LengthSquared() >= 160f * 160f;
    }
}