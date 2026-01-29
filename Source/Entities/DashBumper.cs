using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/DashBumper")]
[TrackedAs(typeof(Bumper))]
public class DashBumper : Bumper {
    public bool SnapDirection;
    public bool Red;
    public DashBumper(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        Red = data.Bool("red", false);
        SnapDirection = data.Bool("snapDirection", false);
        Components.Get<PlayerCollider>().OnCollide = OnPlayer;
    }
    
    private void HotBump(Player player)
    {
        Vector2 vector = (player.Center - base.Center).SafeNormalize();
        hitDir = -vector;
        hitWiggler.Start();
        Audio.Play("event:/game/09_core/hotpinball_activate", Position);
        respawnTimer = 0.6f;
        player.Die(vector);
        SceneAs<Level>().Particles.Emit(P_FireHit, 12, base.Center + vector * 12f, Vector2.One * 3f, vector.Angle());

    }
    private void Bump(Player player)
    {
        Audio.Play("event:/game/06_reflection/pinballbumper_hit", Position);
        respawnTimer = 0.6f;
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        Celeste.Freeze(0.1f);
        Vector2 dir = (player.Center - base.Center).SafeNormalize(-Vector2.UnitY);
        if (SnapDirection)
        {
            dir = Utils.BumperSnapDir(dir, snapUp: false, sidesOnly: false);
        }
        player.ForceDash(dir, Red);
        if(!player.Inventory.NoRefills)

        {
            player.RefillDash();
        }
        player.RefillStamina();
        sprite.Play("hit", restart: true);
        spriteEvil.Play("hit", restart: true);
        light.Visible = false;
        bloom.Visible = false;
        SceneAs<Level>().DirectionalShake(dir, 0.15f);
        SceneAs<Level>().Displacement.AddBurst(base.Center, 0.3f, 8f, 32f, 0.8f);
        SceneAs<Level>().Particles.Emit(P_Launch, 12, base.Center + dir * 12f, Vector2.One * 3f, dir.Angle());
    }
    new public void OnPlayer(Player player)
    {
        if (fireMode)
        {
            if (!SaveData.Instance.Assists.Invincible)
            {
                HotBump(player);
            }
        }
        else if (respawnTimer <= 0f)
        {
            Bump(player);
        }
    }
}