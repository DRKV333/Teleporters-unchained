using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TPUnchained.Tracking;
using TPUnchained.Tiles;
using Terraria;

namespace TPUnchained
{
    public class TPUnchained : Mod
    {
        public enum ModMessageID { RequestLock, RequestUnlock, RequestPush, ShareTeleporterData }

        public TPUnchained()
        {
            Properties = new ModProperties()
            {
                Autoload = true
            };
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModMessageID msg = (ModMessageID)reader.ReadByte();

            int id = reader.ReadInt32();

            TileEntity TE;
            if (!TileEntity.ByID.TryGetValue(id, out TE))
            {
                GetModWorld<TPTrackerWorld>().teleporters.RemoveAll(x => x.ID == id);
                return;
            }

            TEWirelessTeleporter TEw = (TEWirelessTeleporter)TE;

            if (Main.netMode == 2)
            {
                switch (msg)
                {
                    case ModMessageID.RequestLock:
                        TEw.Connect();
                        break;
                    case ModMessageID.RequestUnlock:
                        TEw.Disconnect();
                        break;
                    case ModMessageID.RequestPush:
                        TEw.PushDown();
                        break;
                }
            }
            else if (Main.netMode == 1 && msg == ModMessageID.ShareTeleporterData)
            {
                TEw.NetReceive(reader, false);
            }
        }
    }
}