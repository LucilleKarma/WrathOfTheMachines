﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WoTM.Content.NPCs.ExoMechs.Ares;

public class AresGlowmaskLightPresetRegistry : ModSystem
{
    private static readonly List<AresGlowmaskLightPreset> presets = [];

    public override void OnModLoad()
    {
        // Unpleasant Gradient.
        RegisterNew(1f, () => Main.zenithWorld, [Color.Magenta, Color.Magenta, Color.SaddleBrown, Color.SaddleBrown, Color.SaddleBrown, Color.Lime, Color.Lime]);

        // Audacity logo colors.
        RegisterNew(2f, () =>
        {
            bool connor = Main.LocalPlayer.name.Equals("Connor", StringComparison.OrdinalIgnoreCase);
            bool dronnor = Main.LocalPlayer.name.Equals("Dronnor", StringComparison.OrdinalIgnoreCase);
            return connor || dronnor;
        }, [new Color(0, 67, 235), new Color(250, 195, 24), new Color(255, 24, 2)]);

        // Blue colors.
        RegisterNew(2f, () =>
        {
            return Main.LocalPlayer.name.Equals("midnight", StringComparison.OrdinalIgnoreCase);
        }, [new Color(65, 105, 225), new Color(167, 199, 231), new Color(25, 25, 112), new Color(135, 206, 235), new Color(100, 149, 237)]);

        // Baby blue/light pink. You know the drill.
        RegisterNew(2f, () =>
        {
            return Main.LocalPlayer.name.Equals("Lucille", StringComparison.OrdinalIgnoreCase);
        }, [Color.Cyan, new Color(5, 93, 241), Color.Violet, Color.Turquoise, Color.White]);
    }

    /// <summary>
    /// Attempts to choose an overriding preset, returning null if none are available.
    /// </summary>
    /// <returns></returns>
    public static Color[]? ChooseOverridingPreset()
    {
        var possiblePresets = presets.Where(p => p.Condition()).OrderByDescending(p => p.Priority);
        return possiblePresets.FirstOrDefault()?.LightColors ?? null;
    }

    /// <summary>
    /// Registers a new Ares light glowmask preset.
    /// </summary>
    /// <param name="priority">The priority of this preset.</param>
    /// <param name="condition">The condition that dictates whether the preset is applied.</param>
    /// <param name="lightColors">The palette that Ares should use for his glowmask with this preset.</param>
    public static void RegisterNew(float priority, Func<bool> condition, params Color[] lightColors) =>
        presets.Add(new(priority, condition, lightColors));
}
