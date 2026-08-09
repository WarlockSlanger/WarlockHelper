using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.WarlockHelper.Components;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/MomentumGlider")]
public class MomentumGlider : CustomGlider
{
    //Do I need this much Customizability? It can't hurt much but... Oh well I already started just Finish it.
    public Vector2 ThrowSpeed, ThrowRecoil,
        DropSpeed, DropRecoil,
        PickupBoost; //assuming right facing
    
    public Vector2 ThrowPlayerMass,ThrowGliderMass,
        DropPlayerMass, DropGliderMass,
        PickupPlayerMass, PickupGliderMass;
    
    public bool PlayerBoost, GliderBoost;

    private Sprite spriteOver;

    public MomentumGlider(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        PlayerBoost = data.Bool("playerBoost", true);
        GliderBoost = data.Bool("gliderBoost", true);
        ThrowSpeed = data.Vector2Grouped("throwGliderSpeed", new Vector2(100f,-40f));
        ThrowRecoil = data.Vector2Grouped("throwPlayerSpeed", new Vector2(-80f,0f));
        DropSpeed = data.Vector2Grouped("dropGliderSpeed", Vector2.Zero);
        DropRecoil = data.Vector2Grouped("dropPlayerSpeed", Vector2.Zero);
        PickupBoost = data.Vector2Grouped("pickupSpeed", Vector2.Zero);
        ThrowPlayerMass = data.Vector2Grouped("throwPlayerMomentum", Vector2.One);
        ThrowGliderMass = data.Vector2Grouped("throwGliderMomentum", 0.6f * Vector2.One);
        DropPlayerMass = data.Vector2Grouped("dropPlayerMomentum", Vector2.One);
        DropGliderMass = data.Vector2Grouped("dropGliderMomentum", Vector2.Zero);
        PickupPlayerMass = data.Vector2Grouped("pickupPlayerMomentum", Vector2.One);
        PickupGliderMass = data.Vector2Grouped("pickupGliderMomentum", Vector2.One);
        
        Remove(sprite);
        Add(sprite = GFX.SpriteBank.Create("warlockHelper_momentumGlider"));
        Add(spriteOver = GFX.SpriteBank.Create("warlockHelper_chargeOverlay"));
        
        CHold.throwSpeed = () =>
        {
            Vector2 mass;
            Vector2 speed;
            bool thrown = CHold.ThrowType == CustomHoldable.Throw.Throw;
            mass = thrown ? ThrowGliderMass : DropGliderMass;
            speed = thrown ? ThrowSpeed : DropSpeed;
            speed.X *= CHold.Dir;
            return CHold.Holder.Speed * mass + speed;
        };
        CHold.throwRecoil = () =>
        {
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
            Vector2 playerSpeed=PlayerBoost ? CHold.Holder.GliderBoost(true) : CHold.Holder.Speed; //ik this is confusing What else can i call it

            Vector2 gliderSpeed = Speed;
            if (ChargedSpeed(gliderSpeed))
            {
                Audio.Play("event:/game/01_forsaken_city/birdbros_thrust", Position);
            }
            if (GliderBoost && gliderSpeed.Y is > 0f and < 30f)
            {
                gliderSpeed.Y = 0f;
            }

            Vector2 speedBoost = PickupBoost;
            speedBoost.X *= CHold.Dir;
            return (playerSpeed*PickupPlayerMass + gliderSpeed*PickupGliderMass) + speedBoost;
        };
        //Hold.OnPickup = CustomOnPickup;
        //Hold.OnRelease = CustomOnRelease;
    }
    
    public override void Update()
    {
        base.Update();
        if (destroyed)
        {
            return;
        }
        bool charged = ChargedSpeed(ChargeSpeed);
        if (charged && !spriteOver.Animating)
        {
            spriteOver.Play("on");
        }

        spriteOver.Scale = sprite.Scale;
        spriteOver.Rotation = spriteOver.Rotation;
        if (Scene.OnInterval(charged ? 0.05f : 0.1f))
        {
            float num = Calc.Random.NextAngle();
            SceneAs<Level>().Particles.Emit(ParticleTypes.SparkyDust, 1, Center + new Vector2(0f,-5f) + Calc.AngleToVector(num, 8f), Vector2.One * 2f, num);
        }
    }
    private Vector2 ChargeSpeed => Hold.IsHeld ? (ThrowGliderMass!=Vector2.Zero ? Hold.Holder.Speed : Vector2.Zero) : (PickupGliderMass!=Vector2.Zero ? Speed : Vector2.Zero);
    private bool ChargedSpeed(Vector2 speed)
    {
        speed.X *= 1.4f;
        return speed.LengthSquared() >= 160f * 160f;
    }
}