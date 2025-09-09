using System;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;

namespace Compass {
  class BlockOriginCompass : BlockCompass {
    public override float GetNeedle2DAngleRadians(BlockPos fromPos, ItemStack compassStack) {
      return Get2DAngleRadians(fromPos, GetTargetPos(compassStack));
    }

    public override BlockPos GetTargetPos(ItemStack compassStack) {
      return api.World.DefaultSpawnPosition.AsBlockPos;
    }

    public override void SetTargetPos(ItemStack compassStack, BlockPos targetPos) {
      return;
    }
  }
}