using Terraria.ModLoader;

namespace WoTM.Core.CrossCompatibility;

public class FargosCompatibility : ModSystem
{
    /// <summary>
    /// The Fargos Souls mod.
    /// </summary>
    public static Mod? FargosSouls
    {
        get;
        private set;
    }

    /// <summary>
    /// The Fargos DLC mod.
    /// </summary>
    public static Mod? FargosDLC
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether Eternity Mode is active or not.
    /// </summary>
    public static bool EternityModeIsActive => (bool)(FargosSouls?.Call("Emode") ?? false);

    public override void PostSetupContent()
    {
        if (ModLoader.TryGetMod("FargowiltasSouls", out Mod souls))
            FargosSouls = souls;
        if (ModLoader.TryGetMod("FargowiltasCrossmod", out Mod dlc))
            FargosDLC = dlc;
    }
}
