using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WoTM.Core.CrossCompatibility;

namespace WoTM.Core.BehaviorOverrides
{
    public class NPCOverrideGlobalManager : GlobalNPC
    {
        /// <summary>
        /// The relationship of NPC ID to corresponding override.
        /// </summary>
        internal static readonly Dictionary<int, NPCBehaviorOverride> NPCOverrideRelationship = [];

        /// <summary>
        /// The behavior override that governs the behavior of a given NPC.
        /// </summary>
        internal NPCBehaviorOverride? BehaviorOverride;

        /// <summary>
        /// Whether override effects are permitted by this mod.
        /// </summary>
        internal static bool OverridesPermitted => !InfernumModeCompatibility.InfernumModeIsActive && !ModLoader.HasMod("FargowiltasCrossmod");

        public override bool InstancePerEntity => true;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (NPCOverrideRelationship.TryGetValue(npc.type, out NPCBehaviorOverride? behaviorOverride))
            {
                BehaviorOverride = behaviorOverride!.Clone(npc);
                BehaviorOverride.OnSpawn(source);
            }
        }

        public override void SetDefaults(NPC entity)
        {
            if (!OverridesPermitted)
                return;

            BehaviorOverride?.SetDefaults();
        }

        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            if (NPCOverrideRelationship.TryGetValue(npc.type, out NPCBehaviorOverride? behaviorOverride))
                behaviorOverride?.SetBestiary(database, bestiaryEntry);
        }

        public override bool PreAI(NPC npc)
        {
            if (OverridesPermitted && BehaviorOverride is not null)
            {
                BehaviorOverride.AI();
                return false;
            }

            return true;
        }

        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (!OverridesPermitted)
                return;

            BehaviorOverride?.FindFrame(frameHeight);
        }

        public override void BossHeadSlot(NPC npc, ref int index)
        {
            if (!OverridesPermitted)
                return;

            BehaviorOverride?.BossHeadSlot(ref index);
        }

        public override void ModifyTypeName(NPC npc, ref string typeName)
        {
            if (!OverridesPermitted)
                return;

            BehaviorOverride?.ModifyTypeName(ref typeName);
        }

        public override bool PreKill(NPC npc)
        {
            if (!OverridesPermitted)
                return true;

            if (!(BehaviorOverride?.PreKill() ?? true))
                return false;

            BehaviorOverride?.OnKill();
            return true;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!OverridesPermitted)
                return;

            BehaviorOverride?.ModifyNPCLoot(npcLoot);
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) => BehaviorOverride?.SendExtraAI(bitWriter, binaryWriter);

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) => BehaviorOverride?.ReceiveExtraAI(bitReader, binaryReader);

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!OverridesPermitted)
                return;

            BehaviorOverride?.ModifyHitByProjectile(projectile, ref modifiers);
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (OverridesPermitted && BehaviorOverride is not null)
                return BehaviorOverride.GetAlpha(drawColor);

            return null;
        }

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (!OverridesPermitted)
                return;

            BehaviorOverride?.HitEffect(hit);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!OverridesPermitted || npc.IsABestiaryIconDummy)
                return true;

            return BehaviorOverride?.PreDraw(spriteBatch, screenPos, drawColor) ?? true;
        }

        public override bool CheckDead(NPC npc)
        {
            if (!OverridesPermitted)
                return true;

            return BehaviorOverride?.CheckDead() ?? true;
        }
    }
}
