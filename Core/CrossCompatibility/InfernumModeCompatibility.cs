using System.Runtime.CompilerServices;
using InfernumMode.Core.GlobalInstances.Systems;
using Terraria.ModLoader;

namespace WoTM.Core.CrossCompatibility;

public class InfernumModeCompatibility : ModSystem
{
    /// <summary>
    /// The Infernum mod.
    /// </summary>
    public static Mod? Infernum
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether Infernum Mode is active or not.
    /// </summary>
    public static bool InfernumModeIsActive
    {
        get => (bool)(Infernum?.Call("GetInfernumActive") ?? false);
        set
        {
            if (Infernum is not null)
                SetInfernumActiveBecauseTheModCallIsntWorking(value);
        }
    }

    public override void PostSetupContent()
    {
        if (ModLoader.TryGetMod("InfernumMode", out Mod inf))
            Infernum = inf;
    }

    [JITWhenModsEnabled("InfernumMode")]
    private static void SetInfernumActiveBecauseTheModCallIsntWorking(bool value)
    {
        // The public InfernumModeEnabled property affects the activity state
        // of the sentinels, making them despawn if the property transform to
        // true.
        // Consequently, it is necessary to use the private backing field, to prevent
        // CV and Signus from being deleted from existence every frame.
        [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "infernumModeEnabled")]
        extern static ref bool GetSetInfernumModeEnabled(WorldSaveSystem? system);

        GetSetInfernumModeEnabled(null) = value;
    }
}
