﻿using System;
using CalamityMod.Sounds;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DifferentExoMechs.Content.NPCs.Bosses
{
    public sealed partial class HadesHeadBehaviorOverride : NPCBehaviorOverride
    {
        public float ContinuousLaserBarrage_FireCompletion => Utilities.InverseLerp(0f, ContinuousLaserBarrage_ShootTime, AITimer - ContinuousLaserBarrage_TelegraphTime);

        /// <summary>
        /// How long Hades spends telegraphing and moving around during his ContinuousLaserBarrage attack.
        /// </summary>
        public static int ContinuousLaserBarrage_TelegraphTime => Utilities.SecondsToFrames(2.5f);

        /// <summary>
        /// How long Hades spends shooting lasers during his ContinuousLaserBarrage attack.
        /// </summary>
        public static int ContinuousLaserBarrage_ShootTime => Utilities.SecondsToFrames(2.3f);

        /// <summary>
        /// The standard fly speed at which Hades moves during his ContinuousLaserBarrage attack.
        /// </summary>
        public static float ContinuousLaserBarrage_StandardFlySpeed => 31f;

        /// <summary>
        /// How fast lasers shot by Hades should be during his ContinuousLaserBarrage attack.
        /// </summary>
        public static float ContinuousLaserBarrage_LaserShootSpeed => 17.3f;

        /// <summary>
        /// How close one of Hades' segments has to be to a target in order to fire.
        /// </summary>
        public static float ContinuousLaserBarrage_ShootProximityRequirement => 1085f;

        /// <summary>
        /// AI update loop method for the ContinuousLaserBarrage attack.
        /// </summary>
        public void DoBehavior_ContinuousLaserBarrage()
        {
            if (AITimer == 1)
                SoundEngine.PlaySound(LaserChargeUpSound);

            DoBehavior_ContinuousLaserBarrage_PerformTelegraphing();
            DoBehavior_ContinuousLaserBarrage_FlyAroundTarget();
            DoBehavior_ContinuousLaserBarrage_FireProjectiles();

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void DoBehavior_ContinuousLaserBarrage_PerformTelegraphing()
        {
            float telegraphCompletion = Utilities.InverseLerp(0f, ContinuousLaserBarrage_TelegraphTime, AITimer);
            BodyRenderAction = new(AllSegments(), new(behaviorOverride =>
            {
                float fireCompletion = ContinuousLaserBarrage_FireCompletion;
                float indexRatioAlongHades = behaviorOverride.RelativeIndex / (float)BodySegmentCount;
                float closenessToFiring = Utilities.InverseLerp(-0.1f, 0.01f, fireCompletion - indexRatioAlongHades);
                float fadeOutDueToOncomingFiring = (1f - closenessToFiring).Squared();
                ContinuousLaserBarrage_CreateTelegraphsOnSegments(behaviorOverride, telegraphCompletion, MathF.Sqrt(telegraphCompletion) * fadeOutDueToOncomingFiring * 2485f);
            }));
        }

        public void DoBehavior_ContinuousLaserBarrage_FlyAroundTarget()
        {
            float slowdownFactor = Utils.Remap(AITimer - ContinuousLaserBarrage_TelegraphTime, 0f, 60f, 1f, 0.25f);
            float idealFlySpeed = slowdownFactor * ContinuousLaserBarrage_StandardFlySpeed;
            float newSpeed = MathHelper.Lerp(NPC.velocity.Length(), idealFlySpeed, 0.005f);

            Vector2 flyDestination = Target.Center + Vector2.UnitY * 450f;
            float idealDirection = NPC.AngleTo(flyDestination);
            float currentDirection = NPC.velocity.ToRotation();
            if (NPC.WithinRange(flyDestination, 800f))
                idealDirection = currentDirection;

            NPC.velocity = currentDirection.AngleTowards(idealDirection, 0.1f).ToRotationVector2() * newSpeed;
        }

        public void DoBehavior_ContinuousLaserBarrage_FireProjectiles()
        {
            if (ContinuousLaserBarrage_FireCompletion >= 1f)
                AITimer = 0;

            BodyBehaviorAction = new(AllSegments(), new(behaviorOverride =>
            {
                float fireCompletion = ContinuousLaserBarrage_FireCompletion;
                NPC segment = behaviorOverride.NPC;
                float indexRatioAlongHades = behaviorOverride.RelativeIndex / (float)BodySegmentCount;

                if (behaviorOverride.RelativeIndex % 2 == 0)
                    indexRatioAlongHades *= 0.5f;
                else
                    indexRatioAlongHades = indexRatioAlongHades * 0.5f + 0.5f;

                bool readyToFire = fireCompletion > 0f && MathHelper.Distance(indexRatioAlongHades, fireCompletion) <= 0.01f && behaviorOverride.GenericCountdown <= 0f;
                if (readyToFire && ContinuousLaserBarrage_SegmentCanFire(segment, NPC))
                {
                    Vector2 laserSpawnPosition = behaviorOverride.TurretPosition;
                    SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound with { MaxInstances = 0 }, laserSpawnPosition);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float slowdownFactor = Utils.Remap(Target.Distance(laserSpawnPosition), 775f, 1300f, 0.5f, 1f);
                        Vector2 laserVelocity = (Target.Center - laserSpawnPosition).SafeNormalize(Vector2.UnitY) * slowdownFactor * ContinuousLaserBarrage_LaserShootSpeed;
                        Utilities.NewProjectileBetter(segment.GetSource_FromAI(), laserSpawnPosition, laserVelocity, ModContent.ProjectileType<HadesLaserBurst>(), BasicLaserDamage, 0f, -1, 60f, -1f);

                        behaviorOverride.GenericCountdown = 20f;
                        segment.netUpdate = true;
                    }
                }

                bool hasFired = fireCompletion - indexRatioAlongHades >= 0f;
                if (hasFired)
                    CloseSegment().Invoke(behaviorOverride);
                else
                    OpenSegment(smokeQuantityInterpolant: 0.45f).Invoke(behaviorOverride);
            }));
        }

        /// <summary>
        /// Renders a laser telegraph for a given <see cref="HadesBodyBehaviorOverride"/> in a given direction.
        /// </summary>
        /// <param name="behaviorOverride">The behavior override responsible for the segment.</param>
        public void ContinuousLaserBarrage_CreateTelegraphsOnSegments(HadesBodyBehaviorOverride behaviorOverride, float telegraphCompletion, float telegraphSize)
        {
            if (!ContinuousLaserBarrage_SegmentCanFire(behaviorOverride.NPC, NPC))
                return;

            // TODO -- This is probably bad for performance?
            Main.spriteBatch.PrepareForShaders();

            Vector2 telegraphDirection = behaviorOverride.NPC.SafeDirectionTo(Target.Center);
            RenderLaserTelegraph(behaviorOverride, telegraphCompletion, telegraphSize, telegraphDirection);

            Main.spriteBatch.ResetToDefault();
        }

        /// <summary>
        /// Determines whether a body segment on Hades can fire during the ContinuousLaserBarrage attack.
        /// </summary>
        /// <param name="segment">The segment NPC instance.</param>
        /// <param name="head">The head NPC instance.</param>
        public static bool ContinuousLaserBarrage_SegmentCanFire(NPC segment, NPC head) =>
            segment.WithinRange(Target.Center, ContinuousLaserBarrage_ShootProximityRequirement) && !segment.WithinRange(Target.Center, 300f);
    }
}