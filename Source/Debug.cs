using System;
using System.Collections.Generic;
using Monocle;

namespace Celeste.Mod.WarlockHelper;

internal static class Debug
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

    internal static void DLog(String message)
    {
        Log($"DEBUGLOG WSH {message}");
    }
    internal static void DILog(int val)
    {
        DLog(val.ToString());
    }
    public static componentIds ComponentIds = new(); 
    public class componentIds
    {
        private Dictionary<Component, int> _ids = new();
        private Dictionary<int, Component> _components = new();
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