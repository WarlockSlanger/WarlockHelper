using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/DashDirController")]
[Tracked]

public class DashDirController : Entity
{
    private float Angle;
    public Func<Vector2?> DashDirFunc;
    public DashDirController(EntityData data, Vector2 _) : base()
    {
        Angle = data.Float("angle", 0.0f);
        Angle = MathHelper.ToRadians(Angle);
        DashDirFunc = (() => Input.GetAimVector().Rotate(Angle));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Player player = scene.Tracker.GetEntity<Player>();
        player.SetDefaultDashDir(DashDirFunc);
    }
}