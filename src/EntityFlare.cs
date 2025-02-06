using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Flares;

public class EntityFlare : EntityProjectile
{
    public override void OnCollided()        // Spawn a Flare block
    {
        EntityPos entityPosition = SidedPos;

        int x = (int)entityPosition.X;
        int y = (int)entityPosition.Y;
        int z = (int)entityPosition.Z;


        BlockPos blockPosition = new BlockPos( x, y, z );
       

        World.BlockAccessor.SetBlock(10286, blockPosition);
        Die();
    }

    public override void OnCollideWithLiquid()
    {
       // eventually add in some kind of extinguish effects (maybe pull from torches?)
        Die();
    }
}