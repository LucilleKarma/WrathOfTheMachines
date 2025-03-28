﻿using System;
using WoTM.Common;
using WoTM.Core.Data;

namespace WoTM.Content.NPCs.ExoMechs;

public static class Variables
{
    /// <summary>
    /// Retrives a stored AI integer value with a given name.
    /// </summary>
    /// <param name="name">The value's named key.</param>
    /// <param name="prefix">The file name prefix.</param>
    public static int GetAIInt(string name, ExoMechAIVariableType prefix) => (int)MathF.Round(GetAIFloat(name, prefix));

    /// <summary>
    /// Retrives a stored AI floating point value with a given name.
    /// </summary>
    /// <param name="name">The value's named key.</param>
    /// <param name="prefix">The file name prefix.</param>
    public static float GetAIFloat(string name, ExoMechAIVariableType prefix) =>
        LocalDataManager.Read<DifficultyValue<float>>($"Content/NPCs/ExoMechs/{prefix}AIValues_Standalone.json")[name];
}
