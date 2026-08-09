using Celeste.Mod.WarlockHelper.Triggers;

namespace Celeste.Mod.WarlockHelper;

public class WarlockHelperModuleSession : EverestModuleSession
{
    public DashDirTrigger.PlayerVectorFuncData DefaultDashDirection { get; set; } = null;
}