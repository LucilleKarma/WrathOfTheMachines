using System.IO;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.ArtemisAndApollo;
using WoTM.Core.Networking;

namespace WoTM.Content.NPCs.ExoMechs.Packets
{
    public class ExoMechExoTwinStatePacket : Packet
    {
        public override void Write(ModPacket packet, params object[] context) => ExoTwinsStateManager.SharedState.WriteTo(packet);

        public override void Read(BinaryReader reader) => ExoTwinsStateManager.SharedState.ReadFrom(reader);
    }
}
