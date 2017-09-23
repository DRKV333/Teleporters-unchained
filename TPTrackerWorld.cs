using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TPUnchained.Tiles;
using Microsoft.Xna.Framework;
using Terraria.ID;
using TPUnchained.Items;

namespace TPUnchained
{
    class TPTrackerWorld : ModWorld
    {
        public List<TEWirelessTeleporter> teleporters = new List<TEWirelessTeleporter>();

        public override void Load(TagCompound tag)
        {
            teleporters.Clear();
        }

        public override void PostUpdate()
        {
            if (Main.LocalPlayer.HeldItem.type != mod.ItemType<FineTuningWrenchItem>())
                return;

            Vector2 middleOffset = new Vector2(8, 8);

            if (Main.tileFrameCounter[mod.TileType<WirelessTeleporterTile>()] == 0)
            {
                foreach (var item in teleporters)
                {
                    if(item.Next != Point16.Zero)
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
