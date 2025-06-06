﻿using CalamityMod.NPCs.ExoMechs.Ares;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.SpecificManagers;

namespace WoTM.Content.NPCs.ExoMechs.Projectiles;

public class SmallCannonLaser : ModProjectile, IProjOwnedByBoss<AresBody>, IExoMechProjectile
{
    public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

    public override void SetStaticDefaults() => Main.projFrames[Type] = 4;

    public override void SetDefaults()
    {
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
        Projectile.MaxUpdates = 2;
        Projectile.timeLeft = Projectile.MaxUpdates * 240;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        if (Projectile.IsFinalExtraUpdate())
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Type];
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return LumUtils.CircularHitboxCollision(projHitbox.Center(), Projectile.Size.Length() * 0.5f, targetHitbox);
    }
}
