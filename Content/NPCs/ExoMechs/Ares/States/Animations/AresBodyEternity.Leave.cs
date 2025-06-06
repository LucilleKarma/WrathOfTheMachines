﻿using CalamityMod.NPCs.ExoMechs.Ares;
using Luminance.Common.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using WoTM.Core.BehaviorOverrides;

namespace WoTM.Content.NPCs.ExoMechs.Ares;

public sealed partial class AresBodyBehavior : NPCBehaviorOverride
{
    public void DoBehavior_Leave()
    {
        if (AITimer <= 5)
            IProjOwnedByBoss<AresBody>.KillAll();

        ZPosition = MathHelper.Clamp(ZPosition + 0.12f, 0f, 10f);
        NPC.velocity.X *= 0.932f;
        NPC.velocity.Y -= 0.097f;
        NPC.dontTakeDamage = true;
        NPC.damage = 0;

        BasicHandUpdateWrapper();

        if (ZPosition >= 10f || Main.LocalPlayer.respawnTimer <= 30)
            NPC.active = false;
    }
}
