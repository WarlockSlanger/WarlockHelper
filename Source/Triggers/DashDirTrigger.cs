using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using static Celeste.Mod.WarlockHelper.Utils.Dir8;

namespace Celeste.Mod.WarlockHelper.Triggers;

[CustomEntity("WarlockHelper/DashDirTrigger")]
[Tracked]

public class DashDirTrigger : Trigger
{
    private readonly Func<Player,Vector2> DashDirFunc;
    public bool Persistent;
    public DashDirTrigger(EntityData data, Vector2 offset) : base(data,offset)
    {
        Persistent = data.Bool("persistent");
        bool overrideNeutral = data.Bool("overrideNeutral");
        Vector2 neutralLeft = data.Vector2Grouped("neutralLeft"), neutralRight=data.Vector2Grouped("neutralRight");
        switch (data.Int("_mode"))
        {
            case -1:
            {
                DashDirFunc = null;
                break;
            }
            case 0:
            {
                Utils.Matrix2x2 matrix = new(data.FloatArray("direction", 6, 6, [1, 0, 0, 1, 0, 0]));
                if (!overrideNeutral)
                {
                    neutralLeft = LEFT.Transform(matrix);
                    neutralRight = RIGHT.Transform(matrix);
                }

                DashDirFunc = (player =>
                {
                    if (Input.Aim.Value == Vector2.Zero)
                    {
                        return player.Facing == Facings.Left ? neutralLeft : neutralRight;
                    }

                    return Input.GetAimVector(player.Facing).Transform(matrix);
                });
                break;
            }
            case 1:
            {
                Vector2[] dirs =
                [
                    data.Vector2Grouped("right", RIGHT),
                    data.Vector2Grouped("downRight", DOWNRIGHT),
                    data.Vector2Grouped("down", DOWN),
                    data.Vector2Grouped("downLeft", DOWNLEFT),
                    data.Vector2Grouped("left", LEFT),
                    data.Vector2Grouped("upLeft", UPLEFT),
                    data.Vector2Grouped("up", UP),
                    data.Vector2Grouped("upRight", UPRIGHT)
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
                        return player.Facing == Facings.Left ? neutralLeft : neutralRight;
                    }

                    int angle = Input.GetAimVector(player.Facing).Angle8();
                    return dirs[angle];
                });
                break;
            }
        }
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.SetDefaultDashDir(DashDirFunc);
        if (Persistent)
        {
            WarlockHelperModule.ModSession.DefaultDashDirection = DashDirFunc;
        }
    }
}