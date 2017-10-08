using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TPUnchained.Items;
using TPUnchained.Tiles;

namespace TPUnchained.Tracking
{
    internal class TPTrackerWorld : ModWorld
    {
        public const byte cooldown = 180;

        public List<TEWirelessTeleporter> teleporters = new List<TEWirelessTeleporter>();

        public int[] autoPrev;
        public byte[] autoCooldown;

        public override void Initialize()
        {
            teleporters.Clear();

            autoPrev = new int[Main.player.Length];
            for (int i = 0; i < autoPrev.Length; i++)
            {
                autoPrev[i] = -1;
            }
            autoCooldown = new byte[Main.player.Length];
        }

        public override void PostUpdate()
        {
            for (int i = 0; i < autoCooldown.Length; i++)
            {
                if (autoCooldown[i] > 0)
                    autoCooldown[i]--;

                if (autoCooldown[i] == 0)
                    autoPrev[i] = -1;
            }
        }

        public override void PostDrawTiles()
        {
            if (Main.LocalPlayer.HeldItem.type != mod.ItemType<FineTuningWrenchItem>())
                return;

            Vector2 middleOffset = new Vector2(8, 8);

            if (Main.tileFrameCounter[mod.TileType<WirelessTeleporterTile>()] == 0)
            {
                foreach (var item in teleporters)
                {
                    if (item.Next != Point16.Zero)
                    {
                        Vector2 thisPos = item.Position.ToVector2() * 16 + middleOffset;
                        Vector2 nextPos = item.Next.ToVector2() * 16 + middleOffset;

                        Vector2 velocity = nextPos - thisPos;
                        velocity.Normalize();
                        velocity *= 10;

                        int lifeTime = (int)Vector2.Distance(thisPos, nextPos) / 10;

                        Dust dust = Dust.NewDustPerfect(thisPos, mod.DustType<TracerDust>(), velocity);
                        dust.customData = lifeTime;
                    }
                }
            }
        }
    }
}