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
        private static Dictionary<Component, int> _ids = new Dictionary<Component, int>();
        private static int _currentId = 0;

        public  int this[Component i]
        {
            get
            {
                if (!_ids.TryGetValue(i, out int value))
                {
                    value = _currentId++;
                    _ids.Add(i, value);
                }
                return value;
            }
        }
    }
}