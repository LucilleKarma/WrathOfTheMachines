﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WoTM.Core.Graphics.Models;

// This is FUCKED!
public class ModelRegistry : ModSystem
{
    private static ConstructorInfo contentReaderConstructor;

    private static Func<ContentReader, object> readAsset;

    /// <summary>
    /// The Cargo Plane 3D model that Draedon uses.
    /// </summary>
    public static Model CargoPlane
    {
        get;
        private set;
    }

    public static Model LoadModel(string modelName)
    {
        byte[] modelData = ModContent.GetFileBytes($"WoTM/Assets/Models/{modelName}.xnc");
        using MemoryStream stream = new(modelData);
        return LoadAsset<Model>(new MemoryStream(modelData), modelName);
    }

    private static T LoadAsset<T>(Stream stream, string modelName)
    {
        stream.Seek(10, SeekOrigin.Begin);
        using ContentReader contentReader = (ContentReader)contentReaderConstructor.Invoke([Main.ShaderContentManager, stream, modelName, 0, 'w', null]);
        return (T)readAsset(contentReader)!;
    }

    public override void OnModLoad()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        contentReaderConstructor = typeof(ContentReader).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, [typeof(ContentManager), typeof(Stream), typeof(string), typeof(int), typeof(char), typeof(Action<IDisposable>)])!;
        var readAssetMethod = typeof(ContentReader).GetMethod("ReadAsset", BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(typeof(object));
        readAsset = (Func<ContentReader, object>)Delegate.CreateDelegate(typeof(Func<ContentReader, object>), readAssetMethod);

        Main.QueueMainThreadAction(() =>
        {
            CargoPlane = LoadModel("CargoPlane");
        });
    }
}
