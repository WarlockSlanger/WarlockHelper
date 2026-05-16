using System;
using Celeste.Mod.WarlockHelper.Triggers;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.WarlockHelper;

public class WarlockHelperModuleSession : EverestModuleSession
{
    public DashDirTrigger.PlayerVectorFuncData DefaultDashDirection { get; set; } = null;
}