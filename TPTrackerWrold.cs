using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TPUnchained.Tiles;

namespace TPUnchained
{
    class TPTrackerWrold : ModWorld
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
    }
}
