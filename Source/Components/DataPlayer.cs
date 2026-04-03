using Monocle;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DataPlayer() : Component(active: true, visible: false)
{
    public bool forcedDash = false;
}