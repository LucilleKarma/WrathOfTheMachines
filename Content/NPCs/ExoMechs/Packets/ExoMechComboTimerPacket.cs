using System.IO;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.ComboAttacks;
using WoTM.Core.Networking;

namespace WoTM.Content.NPCs.ExoMechs.Packets
{
    public class ExoMechComboTimerPacket : Packet
    {
        public override void Write(ModPacket packet, params object[] context) =>
            packet.Write(ExoMechComboAttackManager.ComboAttackTimer);

        public override void Read(BinaryReader reader) =>
            ExoMechComboAttackManager.ComboAttackTimer = reader.ReadInt32();
    }
}
