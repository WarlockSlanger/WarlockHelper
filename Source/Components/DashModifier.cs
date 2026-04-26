using Monocle;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DashModifier : Component
{
    public bool silent;
    public bool cooldown;
    public bool changeInterrupt;
    public bool super;

    public DashModifier() : base(active: true, visible: false)
    {
        Reset();
    }
    public void Reset()
    {
        silent = false;
        cooldown = true;
        changeInterrupt = false;
        super = false;
    }
}