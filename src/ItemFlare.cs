using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Flares;

public class ItemFlare : Item 
{
    public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
    {
        base.OnHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling);
        if (handling != EnumHandHandling.PreventDefault)
        {
            handling = EnumHandHandling.PreventDefault;
            byEntity.Attributes.SetInt("aiming", 1);
            byEntity.Attributes.SetInt("aimingCancel", 0);
            byEntity.StartAnimation("aim");
        }
    }

    public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (byEntity.World is IClientWorldAccessor)
        {
            ModelTransform tf = new ModelTransform();
            tf.EnsureDefaultValues();
            float offset = GameMath.Serp(0f, 2f, GameMath.Clamp(secondsUsed * 4f, 0f, 2f) / 2f);
            tf.Translation.Set(0f, offset / 5f, offset / 3f);
            tf.Rotation.Set(offset * 10f, 0f, 0f);
            byEntity.Controls.UsingHeldItemTransformAfter = tf;
        }
        return true;
    }

    public override bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
    {
        byEntity.Attributes.SetInt("aiming", 0);
        byEntity.StopAnimation("aim");
        if (cancelReason != 0)
        {
            byEntity.Attributes.SetInt("aimingCancel", 1);
        }
        return true;
    }

    public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
    {
        if (byEntity.Attributes.GetInt("aimingCancel") == 1)
        {
            return;
        }
        byEntity.Attributes.SetInt("aiming", 0);
        byEntity.StopAnimation("aim");
        if (secondsUsed < 0.35f)
        {
            return;
        }
        (base.api as ICoreClientAPI)?.World.AddCameraShake(0.17f);
        ItemStack stack = slot.TakeOut(1);
        slot.MarkDirty();
        IPlayer byPlayer = null;
        if (byEntity is EntityPlayer player)
        {
            byPlayer = byEntity.World.PlayerByUid(player.PlayerUID);
        }
        byEntity.World.PlaySoundAt(new AssetLocation("sounds/player/throw"), byEntity, byPlayer, randomizePitch: false, 8f);
        EntityProperties entityType = byEntity.World.GetEntityType(Code);
        EntityThrownStone enpr = byEntity.World.ClassRegistry.CreateEntity(entityType) as EntityThrownStone;
        enpr.FiredBy = byEntity;
        enpr.Damage = 0;
        enpr.ProjectileStack = stack;
        /*enpr.DropOnImpactChance = 0;
        enpr.DamageStackOnImpact = false;
        enpr.Weight = 0.3f;*/
        float acc = 1f - byEntity.Attributes.GetFloat("aimingAccuracy");
        double rndpitch = byEntity.WatchedAttributes.GetDouble("aimingRandPitch", 1.0) * (double)acc * 0.75;
        double rndyaw = byEntity.WatchedAttributes.GetDouble("aimingRandYaw", 1.0) * (double)acc * 0.75;
        Vec3d pos = byEntity.ServerPos.XYZ.Add(0.0, byEntity.LocalEyePos.Y - 0.2, 0.0);
        Vec3d velocity = (pos.AheadCopy(1.0, (double)byEntity.ServerPos.Pitch + rndpitch, (double)byEntity.ServerPos.Yaw + rndyaw) - pos) * 0.65;
        Vec3d pos2 = byEntity.ServerPos.BehindCopy(0.21).XYZ.Add(byEntity.LocalEyePos.X, byEntity.LocalEyePos.Y - 0.2, byEntity.LocalEyePos.Z);
        enpr.ServerPos.SetPos(pos2);
        enpr.ServerPos.Motion.Set(velocity);
        enpr.Pos.SetFrom(enpr.ServerPos);
        enpr.World = byEntity.World;
        /*enpr.SetRotation();*/
        byEntity.World.SpawnEntity(enpr);
        byEntity.StartAnimation("throw");
        if (byEntity is EntityPlayer)
        {
            this.RefillSlotIfEmpty(slot, byEntity, (ItemStack itemstack) => itemstack.Collectible is ItemFlare);
        }
        float pitchModifier = (byEntity as EntityPlayer).talkUtil.pitchModifier;
        byPlayer.Entity.World.PlaySoundAt(new AssetLocation("sounds/player/strike"), byPlayer.Entity, byPlayer, pitchModifier * 0.9f + (float)base.api.World.Rand.NextDouble() * 0.2f, 16f, 0.35f);
    }
}