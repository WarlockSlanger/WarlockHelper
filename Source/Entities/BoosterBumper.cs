using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/BoosterBumper")]
public class BoosterBumper : Entity
{
    public const float RadBumper = 12f;

    private static readonly ParticleType P_Ambience = new ParticleType
    {
        Source = GFX.Game["particles/rect"],
        Color = Calc.HexToColor("ff8fc8"),
        Color2 = Calc.HexToColor("cc3d7f"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.InAndOut,
        Size = 0.5f,
        SizeRange = 0.2f,
        RotationMode = ParticleType.RotationModes.SameAsDirection,
        LifeMin = 0.2f,
        LifeMax = 0.4f,
        SpeedMin = 10f,
        SpeedMax = 20f,
        DirectionRange = MathF.PI / 6f
    };

    private static readonly ParticleType P_Launch = new ParticleType
    {
        Source = GFX.Game["particles/rect"],
        Color = Calc.HexToColor("ff8fc8"),
        Color2 = Calc.HexToColor("cc3d7f"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        Size = 0.5f,
        SizeRange = 0.2f,
        RotationMode = ParticleType.RotationModes.Random,
        LifeMin = 0.6f,
        LifeMax = 1.2f,
        SpeedMin = 40f,
        SpeedMax = 140f,
        SpeedMultiplier = 0.1f,
        Acceleration = new Vector2(0f, 10f),
        DirectionRange = MathF.PI / 4.5f
    };

    private Vector2 anchor;
    private bool active=true;

    public bool Red;
    public bool Wobbling;
    public bool SnapDirection;
    public bool SilentDash;
    public bool DashCooldown;
    public bool DashInterrupt;
    public bool DashSuper;
    public bool SnapPosition;
    public bool Respawning;
    
    private Func<Vector2,Vector2> DashDirFunc;
    
    private Sprite Sprite;
    private VertexLight light;
    private BloomPoint bloom;
    private SineWave sine;
    private Alarm alarm;
    
    public BoosterBumper(EntityData data, Vector2 offset)
        : base(data.Position+offset)
    {
        Red = data.Bool("red");
        Wobbling = data.Bool("wobbling");
        SnapDirection = data.Bool("snapDirection");
        SilentDash = data.Bool("silentDash", true);
        DashCooldown = data.Bool("dashCooldown", true);
        DashInterrupt = data.Bool("dashInterrupt");
        DashSuper = data.Bool("dashSuper");
        SnapPosition = data.Bool("snapPosition");

        Depth = data.Int("Depth",Depths.Below);

        float respawnTime = data.Float("respawnTime",0.6f);
        Respawning = respawnTime >= 0f;
        Add(alarm = Alarm.Create(Alarm.AlarmMode.Persist,Reactivate,respawnTime));
        
        Util.Matrix2x2 matrix = new(data.FloatArray("direction",6,6,[1,0,0,1,0,0]));
        DashDirFunc = dir => dir.Transform(matrix);
        
        Collider = new Circle(RadBumper);
        Add(new PlayerCollider(OnPlayer));
        if (Wobbling)
        {
            Add(sine = new SineWave(Bumper.SineCycleFreq, 0f).Randomize());
        }
        Add(Sprite = GFX.SpriteBank.Create("warlockHelper_boosterBumper" + (Red ? "_red" : DashSuper ? "_super" : "")));
        Add(light = new VertexLight(Color.MediumVioletRed, 1f, 16, 32));
        Add(bloom = new BloomPoint(0.5f, 16f));
        
        Vector2? node = data.FirstNodeNullable(offset);
        anchor = Position;
        if (node.HasValue)
        {
            float moveCycleTime = data.Float("moveCycleTime",Bumper.MoveCycleTime);
            Vector2 start = Position;
            Vector2 end = node.Value;
            Tween tween = Tween.Create(Tween.TweenMode.YoyoLooping, Ease.CubeInOut, moveCycleTime, start: true);
            tween.OnUpdate = t =>
            {
                anchor = Vector2.Lerp(start, end, t.Eased);
            };
            Add(tween);
        }
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!Wobbling)
        {
            Position = anchor;
            return;
        }
        Position = anchor + new Vector2(sine.Value * 3.0f, sine.ValueOverTwo * 2.0f);
    }
    public override void Update()
    {
        base.Update();
        if (active)
        {
            float num = Calc.Random.NextAngle();
            ParticleType type = P_Ambience;
            float direction = num;
            float length = 8;
            SceneAs<Level>().Particles.Emit(type, 1, Center + Calc.AngleToVector(num, length), Vector2.One * 2f, direction);
        }
        UpdatePosition();
    }

    private void OnPlayer(Player player)
    {
        active = false;
        Collidable = false;
        Audio.Play("event:/game/06_reflection/pinballbumper_hit", Position);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        Vector2 dir = (player.Center - Center).SafeNormalize(Vector2.UnitY);
        if (SnapDirection)
        {
            dir = dir.EightWayNormal();
        }

        if (SnapPosition)
        {
            Vector2 pos = Position+ dir.SafeNormalize() * RadBumper- player.Collider.Center;
            player.MoveToX(pos.X);
            player.MoveToY(pos.Y);
        }
        dir = DashDirFunc(dir);
        Celeste.Freeze(0.1f);
        player.ForceDash(dashdir: dir, super: DashSuper, red: Red,silent: SilentDash,noCooldown: !DashCooldown,interrupt: DashInterrupt);
        if(!player.Inventory.NoRefills)

        {
            player.RefillDash();
        }
        player.RefillStamina();
        Sprite.Play("hit", restart: true);
        light.Visible = false;
        bloom.Visible = false;
        SceneAs<Level>().DirectionalShake(dir, 0.15f);
        SceneAs<Level>().Displacement.AddBurst(Center, 0.3f, 8f, 32f, 0.8f);
        SceneAs<Level>().Particles.Emit(P_Launch, 12, Center + dir.SafeNormalize() * 12f, Vector2.One * 3f, dir.Angle());
        if (Respawning)
        {
            alarm.Start();
        }
    }
    private void Reactivate()
    {
        active = true;
        Collidable = true;
        light.Visible = true;
        bloom.Visible = true;
        Sprite.Play("on");
        Audio.Play("event:/game/06_reflection/pinballbumper_reset", Position);
    }
}