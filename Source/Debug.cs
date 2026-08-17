using System;
using System.Collections.Generic;
using Monocle;

namespace Celeste.Mod.WarlockHelper;

public static class Debug
{
    internal static void Log(String message = null, LogLevel logLevel = LogLevel.Verbose, String specifier = null)
    {
        String tag = WarlockHelperModule.HelperName;
        if (specifier != null)
        {
            tag += $"/{specifier}";
        }

        message ??= "";
        Logger.Log(logLevel,tag,message);
    }

    internal static void ILLog<T>(T val)
    {
        Log($"DEBUGLOG WSH {val?.ToString() ?? "null"}");
    }

    public static readonly componentIds ComponentIds = new(); 
    public class componentIds
    {
        private readonly Dictionary<Component, int> _ids = new();
        private readonly Dictionary<int, Component> _components = new();
        private static int _currentId;

        public int? this[Component i]
        {
            get
            {
                if (i == null)
                {
                    return null;
                }

                if (_ids.TryGetValue(i, out int value)) return value;
                value = _currentId++;
                _ids.Add(i, value);
                _components.Add(value, i);
                return value;
            }
        }

        public Component this[int i]
        {
            get
            {
                _components.TryGetValue(i,out var component);
                return component;
            }
        }
    }
}