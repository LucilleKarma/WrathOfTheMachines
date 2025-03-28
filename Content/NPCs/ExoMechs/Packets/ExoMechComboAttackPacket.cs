using System.IO;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.ComboAttacks;
using WoTM.Core.Networking;

namespace WoTM.Content.NPCs.ExoMechs.Packets;

public class ExoMechComboAttackPacket : Packet
{
    public override void Write(ModPacket packet, params object[] context)
    {
        int comboAttackIndex = ExoMechComboAttackManager.RegisteredComboAttacks.IndexOf(ExoMechComboAttackManager.CurrentState);
        packet.Write(comboAttackIndex);
    }

    public override void Read(BinaryReader reader)
    {
        int comboAttackIndex = reader.ReadInt32();
        if (comboAttackIndex >= 0 && comboAttackIndex < ExoMechComboAttackManager.RegisteredComboAttacks.Count)
            ExoMechComboAttackManager.CurrentState = ExoMechComboAttackManager.RegisteredComboAttacks[comboAttackIndex];
        else
            ExoMechComboAttackManager.CurrentState = ExoMechComboAttackManager.NullComboState;
    }
}
