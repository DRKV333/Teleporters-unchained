using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TPUnchained.Tiles
{
    internal class TEWirelessTeleporter : ModTileEntity
    {
        public bool isLokced = false;
        public TEWirelessTeleporter Prev { get; private set; }
        public TEWirelessTeleporter Next { get; private set; }

        public override bool ValidTile(int i, int j)
        {
            return Main.tile[i, j].active() && Main.tile[i, j].type == mod.TileType<WirelessTeleporterTile>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3, TileChangeType.None);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            if (Prev != null)
                tag.Add("Prev", Prev.ID);
            if (Next != null)
                tag.Add("Next", Next.ID);
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            TileEntity temp;
            if (tag.ContainsKey("Prev") && ByID.TryGetValue((int)tag["Prev"], out temp))
                Prev = temp as TEWirelessTeleporter;
            if (tag.ContainsKey("Next") && ByID.TryGetValue((int)tag["Next"], out temp))
                Prev = temp as TEWirelessTeleporter;
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            if (Prev != null)
                writer.Write(Prev.ID);
            else
                writer.Write((int)-1);

            if (Next != null)
                writer.Write(Next.ID);
            else
                writer.Write((int)-1);
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            TileEntity temp;
            if (ByID.TryGetValue(reader.ReadInt32(), out temp))
                Prev = temp as TEWirelessTeleporter;
            if (ByID.TryGetValue(reader.ReadInt32(), out temp))
                Prev = temp as TEWirelessTeleporter;
        }
    }
}