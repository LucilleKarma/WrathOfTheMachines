global using LumUtils = Luminance.Common.Utilities.Utilities;
using System.IO;
using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.Ares;
using WoTM.Core.Networking;

namespace WoTM;

public class WoTM : Mod
{
    public override void PostSetupContent()
    {
        BossRushEvent.Bosses.ForEach(b =>
        {
            if (b.EntityID == ModContent.NPCType<Draedon>())
                b.HostileNPCsToNotDelete.Add(ModContent.NPCType<AresHand>());
        });
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketManager.ReceivePacket(reader);
}
