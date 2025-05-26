using System.Linq;
using Terraria;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.FightManagers;

namespace WoTM.Content.NPCs.ExoMechs.SpecificManagers;

public class CustomExoMechsMusicScene : ModSceneEffect
{
    private static int previousTrack = -1;

    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

    public override float GetWeight(Player player) => 0.93f;

    public override int Music => SelectMusic_Jteoh();

    private static int SelectMusic_Jteoh()
    {
        bool artemisApolloActive = ExoMechActive(ExoMechNPCIDs.ApolloID);
        bool aresActive = ExoMechActive(ExoMechNPCIDs.AresBodyID);
        bool hadesActive = ExoMechActive(ExoMechNPCIDs.HadesHeadID);
        if (artemisApolloActive && aresActive && hadesActive)
            return MusicLoader.GetMusicSlot("WoTM/Assets/Sounds/Music/Mayhem");

        if (artemisApolloActive && aresActive)
            return MusicLoader.GetMusicSlot("WoTM/Assets/Sounds/Music/ApolloArtemisAres");
        if (artemisApolloActive && hadesActive)
            return MusicLoader.GetMusicSlot("WoTM/Assets/Sounds/Music/ApolloArtemisHades");
        if (hadesActive && aresActive)
            return MusicLoader.GetMusicSlot("WoTM/Assets/Sounds/Music/HadesAres");

        if (artemisApolloActive)
            return MusicLoader.GetMusicSlot("WoTM/Assets/Sounds/Music/ApolloArtemis");
        if (aresActive)
            return MusicLoader.GetMusicSlot("WoTM/Assets/Sounds/Music/Ares");
        if (hadesActive)
            return MusicLoader.GetMusicSlot("WoTM/Assets/Sounds/Music/Hades");

        return previousTrack;
    }

    private static bool ExoMechActive(int exoMechID) => ExoMechFightStateManager.ActiveManagingExoMechs.Any(e => e.NPCOverrideID == exoMechID);

    public override bool IsSceneEffectActive(Player player)
    {
        bool isActive = ExoMechFightStateManager.FightOngoing && CustomExoMechsSky.Opacity >= 0.05f;

        if (isActive)
        {
            previousTrack = Music;
            return true;
        }

        previousTrack = MusicLoader.GetMusicSlot("CalamityMod/Sounds/Music/DraedonExoSelect");
        return false;
    }
}
