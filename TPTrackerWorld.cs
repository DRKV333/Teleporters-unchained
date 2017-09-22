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

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("IDS"))
            {
                foreach (var item in (int[])tag["IDS"])
                {
                    TileEntity te;
                    if (TileEntity.ByID.TryGetValue(item, out te))
                        teleporters.Add((TEWirelessTeleporter)te);
                }
            }
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            tag.Add("IDS", teleporters.Select(x => x.ID).ToArray());
            return tag;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(teleporters.Count);
            foreach (var item in teleporters.Select(x => x.ID))
            {
                writer.Write(item);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                TileEntity te;
                if (TileEntity.ByID.TryGetValue(reader.ReadInt32(), out te))
                    teleporters.Add((TEWirelessTeleporter)te);
            }
        }

        

        public override void PostUpdate()
        {
            if (Main.tileFrame[mod.TileType<WirelessTeleporterTile>()] == 0 && Main.tileFrameCounter[mod.TileType<WirelessTeleporterTile>()] == 0)
            {
                foreach (var item in teleporters)
                {
                    Vector2 thisPos = new Vector2(item.Position.X * 16 + 8, item.Position.Y * 16 + 8);

                    if(item.Next != -1)
                    {
                        TileEntity next = TileEntity.ByID[item.Next];
                        Vector2 nextPos = new Vector2(next.Position.X * 16 + 8, next.Position.Y * 16 + 8);

                        Dust dust = Dust.NewDustPerfect(thisPos, mod.DustType<TracerDust>(), (nextPos - thisPos) / 10);
                    }
                    
                    if (item.Prev != -1)
                    {
                        TileEntity prev = TileEntity.ByID[item.Prev];
                        Vector2 prevPos = new Vector2(prev.Position.X * 16 + 8, prev.Position.Y * 16 + 8);

                        Dust dust = Dust.NewDustPerfect(prevPos, mod.DustType<TracerDust>(), (thisPos - prevPos) / 10);
                    }
                }
            }
        }
    }
}
