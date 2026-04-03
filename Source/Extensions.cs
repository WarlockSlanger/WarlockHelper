using Microsoft.Xna.Framework;
using Monocle;
using System;
using Celeste.Mod.WarlockHelper.Components;

namespace Celeste.Mod.WarlockHelper;

internal static class Extensions
{
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
            return Utils.TryConvertAll(data.AttrArray(key, minSize,maxSize),out int[] converted,int.TryParse) ? converted : defaultValue;
        }

        public float[] FloatArray(string key, int? minSize = null, int? maxSize = null, float[] defaultValue = null)
        {
            return Utils.TryConvertAll(data.AttrArray(key, minSize,maxSize),out float[] converted,float.TryParse) ? converted : defaultValue;
        }

        public Vector2 Vector2Grouped(string key, Vector2 defaultValue = default)
        {
            float[] arrayVector = data.FloatArray(key, 2,2);
            return arrayVector == null ? defaultValue : new Vector2(arrayVector[0],arrayVector[1]);
        }
    }

    extension(Vector2 vector)
    {
        public Vector2 Transform(Utils.Matrix2x2 matrix)
        {
            return new Vector2(vector.X*matrix.A + vector.Y*matrix.B + matrix.X, vector.X*matrix.C + vector.Y*matrix.D + matrix.Y);
        }

        public int Angle8()
        {
            return ((int)Math.Round(vector.Angle() * 4 / Math.PI)+8)%8;
        }
    }

    extension(Player player)
    {
        public void ForceDash(Vector2? dashdir = null, bool red = false, bool forced = true)
        {
            player.SetNextDashDir(dashdir);
            player.GetSafe<DataPlayer>().forcedDash = forced;
            player.StateMachine.ForceState(red ? Player.StRedDash : Player.StDash);
        }

        public void SetNextDashDir(Vector2? dashdir = null)
        {
            DashDirOverrider ddr = player.GetSafe<DashDirOverrider>();
            ddr.SetCurrent(dashdir);
            Debug.Log(
                $"Forcing next Player dash direction to {dashdir}",LogLevel.Verbose,"SetNextDashDir");
        }

        public void SetDefaultDashDir(Func<Player,Vector2> dashdirfunc)
        {
            DashDirOverrider ddr = player.GetSafe<DashDirOverrider>();
            ddr.SetDefault(dashdirfunc);
            Debug.Log(
                $"Setting default Player dash direction",LogLevel.Verbose,"SetDefaultDashDir");
        }
    }
}