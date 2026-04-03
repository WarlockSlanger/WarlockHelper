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
    private readonly Func<Player,Vector2> DashDirFunc;

    private Player Player;
    public DashDirController(EntityData data, Vector2 _)
    {
        bool overrideNeutral = data.Bool("overrideNeutral");
        Vector2 neutralLeft = data.Vector2Grouped("neutralLeft"), neutralRight=data.Vector2Grouped("neutralRight");
        if (data.Int("_mode") == 0)
        {
            Utils.Matrix2x2 matrix = new (data.FloatArray("direction",6,6,[1,0,0,1,0,0]));
            if (!overrideNeutral)
            {
                neutralLeft = LEFT.Transform(matrix);
                neutralRight = RIGHT.Transform(matrix);
            }
            DashDirFunc = (player =>
            {
                if (Input.Aim.Value == Vector2.Zero)
                {
                    return player.Facing == Facings.Left ? neutralLeft:neutralRight;
                }
                return Input.GetAimVector(player.Facing).Transform(matrix);
            });
            
        }
        else
        {
            Vector2[] dirs =
            [
                data.Vector2Grouped("right",RIGHT),
                data.Vector2Grouped("downRight",DOWNRIGHT),
                data.Vector2Grouped("down",DOWN),
                data.Vector2Grouped("downLeft",DOWNLEFT),
                data.Vector2Grouped("left",LEFT),
                data.Vector2Grouped("upLeft",UPLEFT),
                data.Vector2Grouped("up",UP),
                data.Vector2Grouped("upRight",UPRIGHT)
            ];
            if (!overrideNeutral)
            {
                neutralLeft = dirs[LEFT.Angle8()];
                neutralRight = dirs[RIGHT.Angle8()];
            }
            DashDirFunc = (player =>
            {
                if (Input.Aim.Value == Vector2.Zero)
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