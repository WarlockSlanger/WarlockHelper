using Monocle;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DataPlayer() : Component(active: true, visible: false)
{
    public bool dashInterrupt;
    public bool dashListenersSkipped;
    public bool dashNoCD;
    public bool dashSuper;
}