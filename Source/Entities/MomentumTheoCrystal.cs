using Celeste.Mod.Entities;
using Celeste.Mod.WarlockHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/MomentumTheoCrystal")]
public class MomentumTheoCrystal : CustomTheoCrystal
{
    public Vector2 ThrowSpeed, ThrowRecoil,
        DropSpeed, DropRecoil,
        PickupBoost;
    
    public Vector2 ThrowPlayerMass,ThrowCrystalMass,
        DropPlayerMass, DropCrystalMass,
        PickupPlayerMass, PickupCrystalMass;
    
    public bool CrystalBoost;
    
    public MomentumTheoCrystal(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        Theo = data.Bool("hasTheo", false);
        CrystalBoost = data.Bool("crystalBoost", true);
        ThrowSpeed = data.Vector2Grouped("throwCrystalSpeed", new Vector2(200f,-80f));
        ThrowRecoil = data.Vector2Grouped("throwPlayerSpeed", new Vector2(-80f,0f));
        DropSpeed = data.Vector2Grouped("dropCrystalSpeed", Vector2.Zero);
        DropRecoil = data.Vector2Grouped("dropPlayerSpeed", Vector2.Zero);
        PickupBoost = data.Vector2Grouped("pickupSpeed", Vector2.Zero);
        ThrowPlayerMass = data.Vector2Grouped("throwPlayerMomentum", Vector2.One);
        ThrowCrystalMass = data.Vector2Grouped("throwCrystalMomentum", 0.6f * Vector2.One);
        DropPlayerMass = data.Vector2Grouped("dropPlayerMomentum", Vector2.One);
        DropCrystalMass = data.Vector2Grouped("dropCrystalMomentum", Vector2.Zero);
        PickupPlayerMass = data.Vector2Grouped("pickupPlayerMomentum", Vector2.One);
        PickupCrystalMass = data.Vector2Grouped("pickupCrystalMomentum", Vector2.One);
        
        Remove(sprite);
        Add(sprite = GFX.SpriteBank.Create(Theo ? "warlockHelper_momentumTheo" : "warlockHelper_momentumCrystal"));
        sprite.Scale.X = -1f; //idk why they did this but sure
        
        CHold.throwSpeed = () =>
        {
            if (CHold.ThrowType == CustomHoldable.Throw.Swat)
            {
                return CHold.Holder.Speed * DropCrystalMass + new Vector2(CHold.Dir*160f,-50f);
            }
            Vector2 mass;
            Vector2 speed;
            bool thrown = CHold.ThrowType == CustomHoldable.Throw.Throw;
            mass = thrown ? ThrowCrystalMass : DropCrystalMass;
            speed = thrown ? ThrowSpeed : DropSpeed;
            speed.X *= CHold.Dir;
            return CHold.Holder.Speed * mass + speed;
        };
        CHold.throwRecoil = () =>
        {
            if (CHold.ThrowType == CustomHoldable.Throw.Swat)
            {
                return CHold.Holder.Speed*DropPlayerMass + DropRecoil;
            }
            Vector2 mass;
            Vector2 speed;
            bool thrown = CHold.ThrowType == CustomHoldable.Throw.Throw;
            mass = thrown ? ThrowPlayerMass : DropPlayerMass;
            speed = thrown ? ThrowRecoil : DropRecoil;
            speed.X *= CHold.Dir;
            return CHold.Holder.Speed * mass + speed;
        };
        CHold.pickupSpeed = () =>
        {
            Vector2 playerSpeed=CHold.Holder.Speed; 

            Vector2 crystalSpeed = Speed;
            if (ChargedSpeed(crystalSpeed))
            {
                Audio.Play("event:/game/01_forsaken_city/birdbros_thrust", Position);
            }
            if (CrystalBoost && crystalSpeed.Y is > 0f and < 160f)
            {
                crystalSpeed.Y = 0f;
            }

            Vector2 speedBoost = PickupBoost;
            speedBoost.X *= CHold.Dir;
            return (playerSpeed*PickupPlayerMass + crystalSpeed*PickupCrystalMass) + speedBoost;
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

    private Vector2 ChargeSpeed => Hold.IsHeld ? (ThrowCrystalMass!=Vector2.Zero ? Hold.Holder.Speed : Vector2.Zero) : (PickupCrystalMass!=Vector2.Zero ? Speed : Vector2.Zero);

    private bool ChargedSpeed(Vector2 speed)
    {
        speed.X *= 1.4f;
        return speed.LengthSquared() >= 160f * 160f;
    }
}