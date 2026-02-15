using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/DashDirController")]
[Tracked]

public class DashDirController : Entity
{
    private Utils.Matrix2x2 Matrix;
    public Func<Vector2?> DashDirFunc;

    private Player player;
    public DashDirController(EntityData data, Vector2 _) : base()
    {
        Matrix = new Utils.Matrix2x2(data.Float("a"), data.Float("b"), data.Float("c"), data.Float("d"));
        DashDirFunc = (() => Input.GetAimVector().Transform(Matrix));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        player = scene.Tracker.GetEntity<Player>();
        player.SetDefaultDashDir(DashDirFunc);
        Debug.Log(
            $"Dash Direction Controller from room {this.SourceData.Level.Name} activated",LogLevel.Verbose,"DashDirController.Removed");
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        foreach (Entity entity in scene.Tracker.GetEntities<DashDirController>())
        {
            if (entity != this)
            {
                return;
            }
        }
        player.SetDefaultDashDir(null);
        Debug.Log(
            $"Dash Direction Controller from room {this.SourceData.Level.Name} inactivated", LogLevel.Verbose,
            "DashDirController.Removed");
    }
}