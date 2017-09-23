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
        public Point16 Prev = Point16.Zero;
        public Point16 Next = Point16.Zero;

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
                    if (item.Prev != Point16.Zero)
                    {
                        Prev = item.Prev;
                        Next = item.Position;
                        item.Prev = Position;
                        GetByPos(Prev).Next = Position;
                    }
                    else
                    {
                        item.Prev = Position;
                        item.Next = Position;
                        Prev = item.Position;
                        Next = item.Position;
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
                GetByPos(Prev).Next = Next;
                GetByPos(Next).Prev = Prev;
            }
            else if(Next != Point16.Zero)
            {
                GetByPos(Next).Next = Point16.Zero;
                GetByPos(Next).Prev = Point16.Zero;
            }
            Next = Point16.Zero;
            Prev = Point16.Zero;
            tracker.teleporters.Remove(this);
            isLocked = false;
        }

        public void PushDown()
        {
            if(Prev != Next)
            {
                GetByPos(Prev).Next = Next;
                GetByPos(Next).Prev = Prev;

                Prev = Next;

                Next = GetByPos(Prev).Next;
                GetByPos(Next).Prev = Position;

                GetByPos(Prev).Next = Position;

            }
        }

        public void Teleport()
        {
            if(Next != Point16.Zero)
                Teleport(Position, Next);
            if(Prev != Point16.Zero)
                Teleport(Prev, Position);

            for (int i = 0; i < Main.player.Length; i++)
            {
                Main.player[i].teleporting = false;
            }
            for (int i = 0; i < Main.npc.Length; i++)
            {
                Main.npc[i].teleporting = false;
            }
        }

        private void Teleport(Point16 from, Point16 to)
        {
            Rectangle fromRect = new Rectangle(from.X * 16, from.Y * 16 - 48, 48, 48);
            Rectangle toRect = new Rectangle(to.X * 16, to.Y * 16 - 48, 48, 48);
            Vector2 delta = new Vector2(toRect.X - fromRect.X, toRect.Y - fromRect.Y);

            if (!Wiring.blockPlayerTeleportationForOneIteration)
            {
                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].teleporting && fromRect.Intersects(Main.player[i].getRect()))
                    {
                        Vector2 newPos = Main.player[i].position + delta;
                        Main.player[i].teleporting = true;
                        if (Main.netMode == 2)
                        {
                            RemoteClient.CheckSection(i, newPos, 1);
                        }
                        Main.player[i].Teleport(newPos, 0, 0);
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(65, -1, -1, null, 0, (float)i, newPos.X, newPos.Y, 0, 0, 0);
                        }
                    }
                }
            }

            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].teleporting && Main.npc[i].lifeMax > 5 && !Main.npc[i].boss && !Main.npc[i].noTileCollide)
                {
                    int type = Main.npc[i].type;
                    if (!NPCID.Sets.TeleportationImmune[type] && fromRect.Intersects(Main.npc[i].getRect()))
                    {
                        Main.npc[i].teleporting = true;
                        Main.npc[i].Teleport(Main.npc[i].position + delta, 0, 0);
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

        public TEWirelessTeleporter GetByPos(Point16 pos)
        {
            return (TEWirelessTeleporter)ByPosition[pos];
        }

        public override void OnKill()
        {
            Disconnect();
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            tag.Add("X", Prev.X);
            tag.Add("Y", Prev.Y);
            tag.Add("I", Next.X);
            tag.Add("J", Next.Y);

            tag.Add("L", isLocked);
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("X") && tag.ContainsKey("Y"))
                Prev = new Point16((short)tag["X"], (short)tag["Y"]);
            if (tag.ContainsKey("I") && tag.ContainsKey("J"))
                Next = new Point16((short)tag["I"], (short)tag["J"]);

            if (tag.ContainsKey("L"))
            {
                if ((byte)tag["L"] == 1)
                    isLocked = true;
                else
                    isLocked = false;
            }

            if(isLocked)
                mod.GetModWorld<TPTrackerWorld>().teleporters.Add(this);
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(isLocked);

            writer.WritePackedVector2(Prev.ToVector2());
            writer.WritePackedVector2(Next.ToVector2());
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            isLocked = reader.ReadBoolean();

            Prev = reader.ReadPackedVector2().ToPoint16();
            Next = reader.ReadPackedVector2().ToPoint16();

            if (isLocked)
                mod.GetModWorld<TPTrackerWorld>().teleporters.Add(this);
        }
    }
}