using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WoTM.Content.NPCs.ExoMechs.FightManagers;

namespace WoTM.Content.NPCs.ExoMechs.SpecificManagers;

public class CustomExoMechsMusicScene : ModSceneEffect
{
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

    public override float GetWeight(Player player) => 0.93f;

    public override int Music => MusicLoader.GetMusicSlot("WoTM/Assets/Sounds/Music/HEAVYDUTY");

    public sealed override void SetupContent() => MusicID.Sets.SkipsVolumeRemap[Music] = true;

    public override bool IsSceneEffectActive(Player player)
    {
        MusicID.Sets.SkipsVolumeRemap[Music] = true;
        return ExoMechFightStateManager.FightOngoing && CustomExoMechsSky.Opacity >= 0.05f && false;
    }
}
