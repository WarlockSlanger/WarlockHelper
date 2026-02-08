using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.WarlockHelper;

public static class Debug
{
    public static componentIds ComponentIds = new componentIds(); 
    public class componentIds
    {
        private Dictionary<Component, int> _ids = new();
        private Dictionary<int, Component> _components = new();
        private static int _currentId = 0;

        public  int? this[Component i]
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