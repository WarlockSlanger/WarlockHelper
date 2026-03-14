using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using static Celeste.Mod.WarlockHelper.Utils.Dir8;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/DashDirController")]
[Tracked]

public class DashDirController : Entity
{
    public Func<Player,Vector2> DashDirFunc;
    public bool OverrideNeutral;
    
    private Player Player;
    public DashDirController(EntityData data, Vector2 _)
    {
        OverrideNeutral = data.Bool("overrideNeutral");
        Vector2 neutralLeft = data.Vector2("neutralLeft"), neutralRight=data.Vector2("neutralRight");
        if (data.Has("direction"))
        {
            Utils.Matrix2x2 matrix = new(data.FloatArray("direction"));
            DashDirFunc = ((Player player) =>
            {
                if (OverrideNeutral || Input.Aim.Value == Vector2.Zero)
                {
                    return player.Facing == Facings.Left ? neutralLeft:neutralRight;
                }
                return Input.GetAimVector(player.Facing).Transform(matrix);
            });
            
        }
        else
        {
            Vector2[] dirs =
            {
                data.Vector2("right",RIGHT),
                data.Vector2("downRight",DOWNRIGHT),
                data.Vector2("down",DOWN),
                data.Vector2("downLeft",DOWNLEFT),
                data.Vector2("left",LEFT),
                data.Vector2("upLeft",UPLEFT),
                data.Vector2("up",UP),
                data.Vector2("upRight",UPRIGHT),
            };
            DashDirFunc = ((Player player) =>
            {
                if (OverrideNeutral || Input.Aim.Value == Vector2.Zero)
                {
                    return player.Facing == Facings.Left ? neutralLeft:neutralRight;
                }
                int angle = Input.GetAimVector(player.Facing).Angle8();
                return dirs[angle];
            });
        }
    }

    public DashDirController(Func<Player, Vector2> dashDirFunc)
    {
        DashDirFunc = dashDirFunc;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Player = scene.Tracker.GetEntity<Player>();
        Player.SetDefaultDashDir(DashDirFunc);
        Debug.Log(
            $"Dash Direction Controller from room {SourceData.Level.Name} activated",LogLevel.Verbose,"DashDirController.Removed");
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        foreach (Entity entity in scene.Tracker.GetEntities<DashDirController>())
        {
            if (entity != this)
            {
                return; //reset dashDirection only if there aren't any other controllers
            }
        }
        Player.SetDefaultDashDir(null);
        Debug.Log(
            $"Dash Direction Controller from room {SourceData.Level.Name} deactivated", LogLevel.Verbose,
            "DashDirController.Removed");
    }
}