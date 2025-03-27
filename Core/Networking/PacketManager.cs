using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;

namespace WoTM.Core.Networking;

// NOTE -- This system or more or less equivalent to the one I wrote for Infernum a while ago.
public class PacketManager : ModSystem
{
    internal static Dictionary<string, Packet> RegisteredPackets = [];

    public override void OnModLoad()
    {
        RegisteredPackets = [];
        foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))
        {
            if (!t.IsSubclassOf(typeof(Packet)) || t.IsAbstract)
                continue;

            Packet packet = (Activator.CreateInstance(t) as Packet)!;
            RegisteredPackets[t.FullName!] = packet;
        }
    }

    internal static void PreparePacket(Packet packet, object[] context, short? sender = null)
    {
        // Don't try to send packets in singleplayer.
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;

        // Assume the sender is the current client if nothing else is supplied.
        sender ??= (short)Main.myPlayer;

        ModPacket wrapperPacket = ModContent.GetInstance<WoTM>().GetPacket();

        // Write the identification header. This is necessary to ensure that on the receiving end the reader know how to interpret the packet.
        wrapperPacket.Write(packet.GetType().FullName!);

        // Write the sender and original context if the packet needs to be re-sent from the server.
        if (packet.ResendFromServer)
        {
            wrapperPacket.Write(sender.Value);

            // Send the context data.
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);

            TagCompound tagCompound = new TagCompound();
            for (int i = 0; i < context.Length; i++)
                tagCompound[$"{i}"] = context[i];

            TagIO.Write(tagCompound, writer);

            byte[] contextBytes = stream.ToArray();
            wrapperPacket.Write(context.Length);
            wrapperPacket.Write(contextBytes.Length);
            wrapperPacket.Write(contextBytes);
        }

        // Write the requested packet data.
        packet.Write(wrapperPacket, context);

        // Send the packet.
        wrapperPacket.Send(-1, sender.Value);
    }

    public static void SendPacket<T>(params object[] context) where T : Packet
    {
        // Verify that the packet is registered before trying to send it.
        string packetName = typeof(T).FullName!;
        if (Main.netMode == NetmodeID.SinglePlayer || !RegisteredPackets.TryGetValue(packetName, out Packet? packet))
            return;

        PreparePacket(packet, context);
    }

    public static void ReceivePacket(BinaryReader reader)
    {
        // Read the identification header to determine how the packet should be processed.
        string packetName = reader.ReadString();

        // If no valid packet could be found, get out of here.
        // There will inevitably be a reader underflow error caused by TML's packet policing, but there aren't any clear-cut solutions that
        // I know of that adequately addresses that problem, and from what I can tell it's never catastrophic when it happens.
        if (!RegisteredPackets.TryGetValue(packetName, out Packet? packet))
            return;

        // Determine who sent this packet if it needs to resend.
        short sender = -1;
        object[] context = [];
        if (packet.ResendFromServer)
        {
            sender = reader.ReadInt16();
            int contextLength = reader.ReadInt32();
            int contextByteCount = reader.ReadInt32();
            byte[] contextBytes = reader.ReadBytes(contextByteCount);
            using MemoryStream stream = new MemoryStream(contextBytes);
            using BinaryReader contextReader = new BinaryReader(stream);

            TagCompound tag = TagIO.Read(contextReader);
            context = new object[contextLength];
            for (int i = 0; i < contextLength; i++)
            {
                context[i] = tag.Get<object>($"{i}");
                if (context[i] is TagCompound subTag && subTag.TryGet("x", out int x) && subTag.TryGet("y", out int y))
                    context[i] = new Vector2(x, y);
                else if (context[i] is TagCompound subTag2 && subTag2.TryGet("x", out float x2) && subTag2.TryGet("y", out float y2))
                    context[i] = new Vector2(x2, y2);
            }
        }

        // Read off requested packet data.
        try
        {
            packet.Read(reader);
        }
        catch (Exception e)
        {
            ModContent.GetInstance<WoTM>().Logger.Warn($"Error arose when attempting to process the '{packetName}' packet: {e}");
            throw;
        }

        // If this packet was received server-side and the packet needs to be re-sent, send it back to all the clients, with the
        // exception of the one that originally brought this packet to the server.
        if (Main.netMode == NetmodeID.Server && packet.ResendFromServer)
            PreparePacket(packet, context, sender);
    }
}
