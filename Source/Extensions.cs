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
        public string[] AttrArray(string key, int? size = null, string[] defaultValue = null)
        {
            object obj;
            if (data.Values == null || !data.Values.TryGetValue(key, out obj))
            {
                Debug.Log($"Value {key} not found in {data.Name} {data.ID}", LogLevel.Warn, "AttrArray");
                return defaultValue;
            }

            string valueString = obj.ToString();
            string[] value = valueString.Split(',');
            if (size!=null && value.Length != size)
            {
                throw new FormatException($"Comma-separated array of size {size} required for value {key} of {data.Name} {data.ID}, received {valueString}");
            }
            return value;
        }

        public int[] IntArray(string key, int? size = null, int[] defaultValue = null)
        {
            string[] attrArray = data.AttrArray(key, size);
            return attrArray == null ? defaultValue : Array.ConvertAll(attrArray,int.Parse);
        }

        public float[] FloatArray(string key, int? size = null, float[] defaultValue = null)
        {
            string[] attrArray = data.AttrArray(key, size);
            return attrArray == null ? defaultValue : Array.ConvertAll(attrArray,float.Parse);
        }

        public Vector2 Vector2(string key, Vector2 defaultValue = default)
        {
            float[] arrayVector = data.FloatArray(key, 2);
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
        public void ForceDash(Vector2? dashdir = null, bool red = false)
        {
            SetNextDashDir(player,dashdir);
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
                $"Setting default Player dash direction to {dashdirfunc}",LogLevel.Verbose,"SetDefaultDashDir");
        }
    }
}