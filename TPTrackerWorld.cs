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

namespace TPUnchained
{
    class TPTrackerWorld : ModWorld
    {
        public List<TEWirelessTeleporter> teleporters = new List<TEWirelessTeleporter>();

        public override void PostUpdate()
        {
            Vector2 middleOffset = new Vector2(8, 8);

            if (Main.tileFrame[mod.TileType<WirelessTeleporterTile>()] == 0 && Main.tileFrameCounter[mod.TileType<WirelessTeleporterTile>()] == 0)
            {
                foreach (var item in teleporters)
                {
                    Vector2 thisPos = item.Position.ToVector2() * 16 + middleOffset;

                    if(item.Next != Point16.Zero)
                    {
                        Vector2 nextPos = item.Next.ToVector2() * 16 + middleOffset;
                        Dust dust = Dust.NewDustPerfect(thisPos, mod.DustType<TracerDust>(), (nextPos - thisPos) / 10);
                    }
                }
            }
        }
    }
}
