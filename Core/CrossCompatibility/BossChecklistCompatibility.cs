using System.Reflection;
using Luminance.Core.Hooking;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.FightManagers;

namespace WoTM.Core.CrossCompatibility;

public class BossChecklistCompatibility : ModSystem
{
    /// <summary>
    /// The boss checklist mod.
    /// </summary>
    public static Mod BossChecklistMod
    {
        get;
        private set;
    }

    internal delegate LocalizedText? GetDespawnMessageOrig(object instance, NPC npc);

    internal delegate LocalizedText? GetDespawnMessageHook(GetDespawnMessageOrig orig, object instance, NPC npc);

    public override void PostSetupContent()
    {
        if (!ModLoader.TryGetMod("BossChecklist", out Mod bc))
            return;
        BossChecklistMod = bc;

        ChangeDraedonDespawnMessage();
    }

    [JITWhenModsEnabled("BossChecklist")]
    private void ChangeDraedonDespawnMessage()
    {
        MethodInfo? despawnMessageMethod = BossChecklistMod.Code.GetType("BossChecklist.EntryInfo")?.GetMethod("GetDespawnMessage", LumUtils.UniversalBindingFlags);
        if (despawnMessageMethod is null)
            Mod.Logger.Warn("Could not find Boss Checklist's boss despawn message method.");
        else
        {
            HookHelper.ModifyMethodWithDetour(despawnMessageMethod, new GetDespawnMessageHook((orig, instance, npc) =>
            {
                if (ExoMechNPCIDs.ExoMechIDs.Contains(npc.type))
                    return null;

                return orig(instance, npc);
            }));
        }
    }
}
