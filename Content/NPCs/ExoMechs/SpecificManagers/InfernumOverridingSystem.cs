using Terraria;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.FightManagers;
using WoTM.Core.CrossCompatibility;

namespace WoTM.Content.NPCs.ExoMechs.SpecificManagers;

public class InfernumOverridingSystem : ModSystem
{
    private static bool? infernumWasEnabled;

    // Separate from ExoMechFightStateManager.FightOngoing to ensure that effects don't have a one-frame delay.
    private static bool exosFightIsOngoing;

    public override void OnModLoad()
    {
        On_WorldGen.SaveAndQuitCallBack += EnsureInfernumDoesntGetDisabled;
        On_Main.DrawNPCs += OverrideInfernumNPCRendering;
    }

    private static void EnsureInfernumDoesntGetDisabled(On_WorldGen.orig_SaveAndQuitCallBack orig, object threadContext)
    {
        if (infernumWasEnabled.HasValue)
        {
            InfernumModeCompatibility.InfernumModeIsActive = infernumWasEnabled.Value;
            infernumWasEnabled = null;
        }
        orig(threadContext);
    }

    private static void OverrideInfernumNPCRendering(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
    {
        bool infernumWasEnabled = InfernumModeCompatibility.InfernumModeIsActive;

        try
        {
            if (exosFightIsOngoing)
                InfernumModeCompatibility.InfernumModeIsActive = false;
            orig(self, behindTiles);
        }
        finally
        {
            InfernumModeCompatibility.InfernumModeIsActive = infernumWasEnabled;
        }
    }

    public override void PreUpdateEntities()
    {
        bool draedonIsPresent = NPC.AnyNPCs(ModContent.NPCType<CalamityMod.NPCs.ExoMechs.Draedon>());
        bool anyExoMechs = false;
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (ExoMechNPCIDs.ManagingExoMechIDs.Contains(npc.type))
            {
                anyExoMechs = true;
                break;
            }
        }
        exosFightIsOngoing = draedonIsPresent || anyExoMechs;

        infernumWasEnabled = null;
        if (exosFightIsOngoing)
        {
            infernumWasEnabled = InfernumModeCompatibility.InfernumModeIsActive;
            InfernumModeCompatibility.InfernumModeIsActive = false;
        }
    }

    // Projectiles update after NPCs. It's important that Infernum get reset after both NPCs and Projectiles have updated, to ensure that they
    // both use the correct AI code.
    public override void PostUpdateProjectiles()
    {
        if (infernumWasEnabled.HasValue)
            InfernumModeCompatibility.InfernumModeIsActive = infernumWasEnabled.Value;
    }
}
