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
    public class PlayerVectorFuncData
    {
        public Utils.Matrix2x2 matrix=null;
        public Vector2[] dirs=null;
        public Vector2? neutralLeft = null, neutralRight = null;
    }

    private PlayerVectorFuncData pvf;
    private readonly Func<Player,Vector2> DashDirFunc;
    public bool Persistent;

    internal static Func<Player, Vector2> DataToFunc(PlayerVectorFuncData data)
    {
        if (data?.matrix!=null) {
                Utils.Matrix2x2 matrix = data.matrix;
                Vector2 neutralLeft=data.neutralLeft ?? LEFT.Transform(matrix),
                    neutralRight=data.neutralRight ?? RIGHT.Transform(matrix);

                return (player =>
                {
                    if (Input.Aim.Value == Vector2.Zero)
                    {
                        return player.Facing == Facings.Left ? neutralLeft : neutralRight;
                    }

                    return Input.GetAimVector(player.Facing).Transform(matrix);
                });
        }
        if (data?.dirs!=null) {
            {
                Vector2[] dirs = data.dirs;
                Vector2 neutralLeft=data.neutralLeft ?? dirs[LEFT.Angle8()],
                    neutralRight=data.neutralRight ?? dirs[RIGHT.Angle8()];
                return (player =>
                {
                    if (Input.Aim.Value == Vector2.Zero)
                    {
                        return player.Facing == Facings.Left ? neutralLeft : neutralRight;
                    }

                    int angle = Input.GetAimVector(player.Facing).Angle8();
                    return dirs[angle];
                });
            }
        }
        return null;
    }

    public DashDirTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Persistent = data.Bool("persistent");
        bool overrideNeutral = data.Bool("overrideNeutral");
        int mode = data.Int("_mode");
        if (mode == -1)
        {
            pvf = null;
            DashDirFunc = null;
            return;
        }
        pvf = new PlayerVectorFuncData();
        switch (mode)
        {
            case 0:
            {
                pvf.matrix = new(data.FloatArray("direction", 6, 6, [1, 0, 0, 1, 0, 0]));
                break;
            }
            case 1:
            {
                pvf.dirs =
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
                break;
            }
        }
        pvf.neutralLeft = overrideNeutral ? data.Vector2Grouped("neutralLeft") : null;
        pvf.neutralRight=overrideNeutral ? data.Vector2Grouped("neutralRight") : null;
        DashDirFunc = DataToFunc(pvf);
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.SetDefaultDashDir(DashDirFunc);
        if (Persistent)
        {
            WarlockHelperModule.ModSession.DefaultDashDirection = pvf;
        }
    }
}