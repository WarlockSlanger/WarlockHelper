using Microsoft.Xna.Framework;
using Celeste.Mod.WarlockHelper.Components;

namespace Celeste.Mod.WarlockHelper.Entities;

public abstract class CustomGlider : Glider 
{
    public CustomHoldable CHold;
    
    public CustomGlider(EntityData data,Vector2 offset) : base(data,offset)
    {
        Add(CHold = new CustomHoldable());
    }
}