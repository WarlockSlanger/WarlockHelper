using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/DashBumper")]
public class DashBumper : Entity 
{
    public ParticleType P_Ambience = Bumper.P_Ambience;
    public ParticleType P_Launch = Bumper.P_Launch;
    public Sprite sprite;
    public VertexLight light;
    public BloomPoint bloom;
    public Vector2? node;
    public Vector2 anchor;
    public SineWave sine;
    public float respawnTimer;

    public float RespawnTime;
    public bool Red;
    public bool Wobbling;
    public bool SnapDirection;
    
    private float MoveCycleTime;
    private float SineCycleFreq;
    private float DashSpeed;
    
    public DashBumper(EntityData data, Vector2 offset)
        : base(data.Position+offset)
    {
        Collider = new Circle(12f);
        Add(new PlayerCollider(OnPlayer));
        Add(sine = new SineWave(SineCycleFreq, 0f).Randomize());
        Add(sprite = GFX.SpriteBank.Create("bumper"));
        Add(light = new VertexLight(Color.Teal, 1f, 16, 32));
        Add(bloom = new BloomPoint(0.5f, 16f));
        node = data.FirstNodeNullable(offset);anchor = Position;
        if (node.HasValue)
        {
            Vector2 start = Position;
            Vector2 end = node.Value;
            Tween tween = Tween.Create(Tween.TweenMode.YoyoLooping, Ease.CubeInOut, MoveCycleTime, start: true);
            tween.OnUpdate = (Tween t) =>
            {
                anchor = Vector2.Lerp(start, end, t.Eased);
            };
            Add(tween);
        }
        UpdatePosition();
        Red = data.Bool("red");
        Wobbling = data.Bool("wobbling");
        SnapDirection = data.Bool("snapDirection");
        RespawnTime = data.Float("respawnTime",0.6f);
        MoveCycleTime = data.Float("moveCycleTime",1.8181819f);
        SineCycleFreq = data.Float("sineCycleFreq",0.44f);
        DashSpeed = data.Float("dashSpeed",240f);
        DashSpeed /= 240f;
    }
    private void UpdatePosition()
    {
        if (!Wobbling)
        {
            Position = anchor;
        }
        Position = anchor + new Vector2(sine.Value * 3.0f, sine.ValueOverTwo * 2.0f);
    }
    public override void Update()
    {
        base.Update();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
            {
                light.Visible = true;
                bloom.Visible = true;
                sprite.Play("on");
                Audio.Play("event:/game/06_reflection/pinballbumper_reset", Position);
            }
        }
        else if (Scene.OnInterval(0.05f))
        {
            float num = Calc.Random.NextAngle();
            ParticleType type = P_Ambience;
            float direction = num;
            float length = 8;
            SceneAs<Level>().Particles.Emit(type, 1, Center + Calc.AngleToVector(num, length), Vector2.One * 2f, direction);
        }
        UpdatePosition();
    }
    public void OnPlayer(Player player)
    {
        if (respawnTimer <= 0f)
        {
            Audio.Play("event:/game/06_reflection/pinballbumper_hit", Position);
            respawnTimer = RespawnTime;
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            Celeste.Freeze(0.1f);
            Vector2 dir = (player.Center - Center).SafeNormalize(Vector2.UnitY);
            if (SnapDirection)
            {
                dir = Utils.DashDirSnap(dir);
            }
            player.ForceDash(DashSpeed*dir, Red);
            if(!player.Inventory.NoRefills)

            {
                player.RefillDash();
            }
            player.RefillStamina();
            sprite.Play("hit", restart: true);
            light.Visible = false;
            bloom.Visible = false;
            SceneAs<Level>().DirectionalShake(dir, 0.15f);
            SceneAs<Level>().Displacement.AddBurst(Center, 0.3f, 8f, 32f, 0.8f);
            SceneAs<Level>().Particles.Emit(P_Launch, 12, base.Center + dir * 12f, Vector2.One * 3f, dir.Angle());
        }
    }
}