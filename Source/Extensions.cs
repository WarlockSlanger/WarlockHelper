using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.WarlockHelper.Components;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Celeste.Mod.WarlockHelper;

public static class Extensions
{
    extension(Level level)
    {
        internal void RespawnNewRoom(string Level, Vector2? RespawnPoint=null,  Vector2? DieVector=null, Player.IntroTypes IntroType=Player.IntroTypes.Respawn, float delay=0f, Player player = null)
        {
            player ??= level.Tracker.GetEntity<Player>();
            Leader.StoreStrawberries(player.Leader);
            PlayerDeadBody playerDeadBody = player.Die((DieVector??Vector2.Zero).SafeNormalize(), evenIfInvincible: true,
                registerDeathInStats: false);
            playerDeadBody.DeathAction = () =>
            {
                level.OnEndOfFrame += () =>
                {
                    level.UnloadLevel();
                    level.Session.Level = Level;
                    level.Session.RespawnPoint = level.GetSpawnPoint(RespawnPoint ?? new Vector2(level.Bounds.Left, level.Bounds.Top));
                    level.Session.FirstLevel = false;
                    level.LoadLevel(IntroType);
                    Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
                };
            };
            playerDeadBody.ActionDelay = delay;
        }

        internal void LoadNewRoom(string Level, Vector2? RespawnPoint=null, Player player = null)
        {
            player ??= level.Tracker.GetEntity<Player>();
            level.OnEndOfFrame += () =>
            {
                Vector2 oldLevelOffset = level.LevelOffset;
                Vector2 vector = player.Position - oldLevelOffset;
                Vector2 vector2 = level.Camera.Position - oldLevelOffset;
                level.Remove(player);
                level.UnloadLevel();
                level.Session.Level = Level;
                level.Session.RespawnPoint = level.GetSpawnPoint(RespawnPoint ?? new Vector2(level.Bounds.Left, level.Bounds.Top));
                level.Session.FirstLevel = false;
                level.LoadLevel(Player.IntroTypes.Transition);
                level.Camera.Position = level.LevelOffset + vector2;
                level.Add(player);
                player.Position = level.LevelOffset + vector;
                player.Hair.MoveHairBy(level.LevelOffset - oldLevelOffset);
            };
        }
    }
    extension(Entity entity)
    {
        public T GetSafe<T>() where T : Component, new()
        {
            T comp = entity.Get<T>();
            if (comp == null)
            {
                comp = new T();
                entity.Add(comp);
            }
            return comp;
        }
    }
    extension(EntityData data)
    {
        private string[] AttrArray(string key, int? minSize = null, int? maxSize=null, string[] defaultValue = null)
        {
            if (data.Values == null || !data.Values.TryGetValue(key, out var obj) || obj is not string)
            {
                Debug.Log($"String \"{key}\" not found in {data.Name} with ID {data.ID}", LogLevel.Warn, "StrArray");
                return defaultValue;
            }
            string valueString = obj.ToString();
            string[] value = valueString!.Split(',');
            if ((minSize!=null && value.Length<minSize) || (maxSize!=null && value.Length > maxSize))
            {
                Debug.Log($"Comma-separated array of size from {minSize} to {maxSize} required for value \"{key}\" of {data.Name} with ID {data.ID}, received {valueString}",LogLevel.Warn,"StrArray");
                return defaultValue;
            }
            return value;
        }
        public int[] IntArray(string key, int? minSize = null, int? maxSize=null, int[] defaultValue = null)
        {
            return Util.TryConvertAll(data.AttrArray(key, minSize,maxSize),out int[] converted,int.TryParse) ? converted : defaultValue;
        }

        public float[] FloatArray(string key, int? minSize = null, int? maxSize = null, float[] defaultValue = null)
        {
            return Util.TryConvertAll(data.AttrArray(key, minSize,maxSize),out float[] converted,float.TryParse) ? converted : defaultValue;
        }

        public Vector2 Vector2Grouped(string key, Vector2 defaultValue = default)
        {
            float[] arrayVector = data.FloatArray(key, 2,2);
            return arrayVector == null ? defaultValue : new Vector2(arrayVector[0],arrayVector[1]);
        }
    }

    extension(Vector2 vector)
    {
        internal Vector2 Transform(Util.Matrix2x2 matrix)
        {
            return new Vector2(vector.X*matrix.A + vector.Y*matrix.B + matrix.X, vector.X*matrix.C + vector.Y*matrix.D + matrix.Y);
        }

        public int Angle8()
        {
            return Util.mod((int)Math.Round(vector.Angle() * 8 / Math.Tau),8);
        }
        public Vector2 SnapDirection(int resolution=8,float center=0f)
        {
            float unit = (float)Math.Tau / resolution;
            float angle = vector.Angle()-center;
            angle = (float)Math.Round(angle / unit)*unit;
            angle += center;
            return Util.fromAngle(vector.Length(), angle);
        }
    }

    extension(Player player)
    {
        public void ForceDash(Vector2? dashdir = null,bool super=false, bool red = false, bool silent= true, bool noCooldown=false, bool interrupt = false)
        {
            var dmod = player.GetSafe<DashModifier>();
            player.SetNextDashDir(dashdir);
            dmod.noCooldown = noCooldown;
            dmod.silent = silent;
            dmod.changeInterrupt = red^interrupt;
            dmod.super = super;
            player.StateMachine.ForceState(red ? Player.StRedDash : Player.StDash);
        }

        public void SetNextDashDir(Vector2? dashdir = null)
        {
            DashModifier ddr = player.GetSafe<DashModifier>();
            ddr.SetCurrent(dashdir);
        }

        public void SetDefaultDashDir(Func<Player,Vector2> dashdirfunc)
        {
            DashModifier ddr = player.GetSafe<DashModifier>();
            ddr.SetDefault(dashdirfunc);
        }

        public Vector2 GliderBoost(bool sideEffects=false)
        {
            Vector2 ans = player.Speed;
            ans.Y = Math.Min(ans.Y, 0f);
            if (player.gliderBoostTimer > 0f && player.gliderBoostDir.Y < 0f)
            {
                if (sideEffects)
                {
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
                }
                player.gliderBoostTimer = 0f;
                ans.Y = Math.Min(ans.Y, -240f * Math.Abs(player.gliderBoostDir.Y));
            }
            else if (ans.Y < 0f)
            {
                ans.Y = Math.Min(ans.Y, -105f);
            }

            return ans;
        }
    }

    extension(ILCursor cursor)
    {
        internal void emitThis(bool coroutine = false)
        {
            if (coroutine)
            {
                cursor.EmitLdloc1();
            }
            else
            {
                cursor.EmitLdarg0();
            }
        }
    }

    extension(Instruction instr)
    {
        internal bool compGeneric(Type[] types)
        {
            if (instr.Operand is not GenericInstanceMethod g)
            {
                throw new ArgumentException("Instruction operand is not a method");
            }
            var args = g.GenericArguments;
            int n = types.Length;
            if (args.Count != n)
            {
                return false;
            }

            for (int i = 0; i < n; i++)
            {
                if (types[i].FullName != args[i].FullName)
                {
                    return false;
                }
            }

            return true;
        }
    }
}