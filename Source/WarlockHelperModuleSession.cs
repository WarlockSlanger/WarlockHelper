using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.WarlockHelper;

public class WarlockHelperModuleSession : EverestModuleSession
{
    public Func<Player, Vector2> DefaultDashDirection { get; set; } = null;
}