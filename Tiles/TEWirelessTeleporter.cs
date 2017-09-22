using Microsoft.Xna.Framework;
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
        public bool isLocked = false;
        public int Prev = -1;
        public int Next = -1;

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

        public void Connect()
        {
            int address = GetAddress();
            TPTrackerWorld tracker = mod.GetModWorld<TPTrackerWorld>();
            foreach (var item in tracker.teleporters)
            {
                if(item.GetAddress() == address)
                {
                    if (item.Prev != -1)
                    {
                        Prev = item.Prev;
                        Next = item.ID;
                        item.Prev = ID;
                        GetByID(Prev).Next = ID;
                    }
                    else
                    {
                        item.Prev = ID;
                        item.Next = ID;
                        Prev = item.ID;
                        Next = item.ID;
                    }
                    break;
                }
            }
            tracker.teleporters.Add(this);
            isLocked = true;
        }

        public void Disconnect()
        {
            TPTrackerWorld tracker = mod.GetModWorld<TPTrackerWorld>();
            if (Prev != Next)
            {
                GetByID(Prev).Next = Next;
                GetByID(Next).Prev = Prev;
            }
            else if(Next != -1)
            {
                GetByID(Next).Next = -1;
                GetByID(Next).Prev = -1;
            }
            Next = -1;
            Prev = -1;
            tracker.teleporters.Remove(this);
            isLocked = false;
        }

        public void PushDown()
        {

        }

        public void Teleport()
        {
            Teleport(Position, GetByID(Next).Position);
            Teleport(GetByID(Prev).Position, Position);

            for (int l = 0; l < Main.player.Length; l++)
            {
                Main.player[l].teleporting = false;
            }
        }

        private void Teleport(Point16 from, Point16 to)
        {
            if (Wiring.blockPlayerTeleportationForOneIteration)
                return;

            Rectangle fromRect = new Rectangle(from.X * 16, from.Y * 16 - 48, 48, 48);
            Rectangle toRect = new Rectangle(to.X * 16, to.Y * 16 - 48, 48, 48);
            Vector2 delta = new Vector2(toRect.X - fromRect.X, toRect.Y - fromRect.Y);


            for (int j = 0; j < Main.player.Length; j++)
            {
                if (Main.player[j].active && !Main.player[j].dead && !Main.player[j].teleporting && fromRect.Intersects(Main.player[j].getRect()))
                {
                    Vector2 newPos = Main.player[j].position + delta;
                    Main.player[j].teleporting = true;
                    if (Main.netMode == 2)
                    {
                        RemoteClient.CheckSection(j, newPos, 1);
                    }
                    Main.player[j].Teleport(newPos, 0, 0);
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(65, -1, -1, null, 0, (float)j, newPos.X, newPos.Y, 0, 0, 0);
                    }
                }
            }
        }

        public int GetAddress()
        {
            int add = Main.tile[Position.X - 1, Position.Y].frameY / 18;
            add |= (Main.tile[Position.X , Position.Y].frameY / 18) << 4;
            add |= (Main.tile[Position.X + 1, Position.Y].frameY / 18) << 8;
            return add;
        }

        public TEWirelessTeleporter GetByID(int ID)
        {
            return (TEWirelessTeleporter)TileEntity.ByID[ID];
        }

        public override void OnKill()
        {
            Disconnect();
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            tag.Add("Prev", Prev);
            tag.Add("Next", Next);

            tag.Add("isLocked", isLocked);
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("Prev") )
                Prev = (int)tag["Prev"];
            if (tag.ContainsKey("Next"))
                Next = (int)tag["Next"];

            if (tag.ContainsKey("isLocked"))
            {
                if ((byte)tag["isLocked"] == 1)
                    isLocked = true;
                else
                    isLocked = false;
            }
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(isLocked);

            writer.Write(Prev);
            writer.Write(Next);
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            isLocked = reader.ReadBoolean();

            Prev = reader.ReadInt32();
            Next = reader.ReadInt32();
        }
    }
}