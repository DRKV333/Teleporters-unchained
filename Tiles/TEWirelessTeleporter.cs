using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TPUnchained.Tracking;

namespace TPUnchained.Tiles
{
    public class TEWirelessTeleporter : ModTileEntity
    {
        private bool autoTrigger = false;

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
            if (isLocked)
                return;

            if (Main.netMode == 1)
            {
                SendRequest(TPUnchained.ModMessageID.RequestLock);
            }
            else
            {
                isLocked = true;

                int address = GetAddress();
                TPTrackerWorld tracker = mod.GetModWorld<TPTrackerWorld>();
                foreach (var item in tracker.teleporters)
                {
                    if (item.GetAddress() == address)
                    {
                        if (item.Prev != Point16.Zero)
                        {
                            Prev = item.Prev;
                            Next = item.Position;
                            item.Prev = Position;
                            GetByPos(Prev).Next = Position;
                            if (Main.netMode == 2)
                            {
                                GetByPos(Prev).SendData();
                                item.SendData();
                            }
                        }
                        else
                        {
                            item.Prev = Position;
                            item.Next = Position;
                            Prev = item.Position;
                            Next = item.Position;
                            if (Main.netMode == 2)
                            {
                                item.SendData();
                            }
                        }

                        if (Main.netMode == 2)
                        {
                            SendData();
                        }

                        break;
                    }
                }
                tracker.teleporters.Add(this);
            }
        }

        public void Disconnect()
        {
            if (!isLocked)
                return;

            if (Main.netMode == 1)
            {
                SendRequest(TPUnchained.ModMessageID.RequestUnlock);
            }
            else
            {
                isLocked = false;

                TPTrackerWorld tracker = mod.GetModWorld<TPTrackerWorld>();
                if (Prev != Next)
                {
                    GetByPos(Prev).Next = Next;
                    GetByPos(Next).Prev = Prev;
                    if (Main.netMode == 2)
                    {
                        GetByPos(Prev).SendData();
                        GetByPos(Next).SendData();
                    }
                }
                else if (Next != Point16.Zero)
                {
                    GetByPos(Next).Next = Point16.Zero;
                    GetByPos(Next).Prev = Point16.Zero;
                    if (Main.netMode == 2)
                    {
                        GetByPos(Next).SendData();
                    }
                }

                Next = Point16.Zero;
                Prev = Point16.Zero;
                tracker.teleporters.Remove(this);

                if (Main.netMode == 2)
                {
                    SendData();
                }
            }
        }

        public void PushDown()
        {
            if (!isLocked)
                return;

            if (Main.netMode == 1)
            {
                SendRequest(TPUnchained.ModMessageID.RequestPush);
            }
            else
            {
                if (Prev != Next)
                {
                    List<TEWirelessTeleporter> toShare = new List<TEWirelessTeleporter>();
                    toShare.Add(this);

                    GetByPos(Prev).Next = Next;
                    toShare.Add(GetByPos(Prev));
                    GetByPos(Next).Prev = Prev;
                    toShare.Add(GetByPos(Next));

                    Prev = Next;

                    Next = GetByPos(Prev).Next;
                    GetByPos(Next).Prev = Position;
                    toShare.Add(GetByPos(Next));

                    GetByPos(Prev).Next = Position;

                    if (Main.netMode == 2)
                    {
                        foreach (var item in toShare)
                        {
                            item.SendData();
                        }
                    }
                }
            }
        }

        public void CheckTriggerState()
        {
            Tile[] tile = new Tile[] { Main.tile[Position.X + 2, Position.Y], Main.tile[Position.X - 2, Position.Y] };
            foreach (var item in tile)
            {
                if (item != null && item.active() && item.type == mod.TileType<TeleporterAutotriggerTile>())
                {
                    autoTrigger = true;
                    return;
                }
            }
            autoTrigger = false;
        }

        public void Teleport()
        {
            if (Next != Point16.Zero)
                Teleport(Position, Next, false);
            if (Prev != Point16.Zero && Prev != Next)
                Teleport(Prev, Position, false);
        }

        private void Teleport(Point16 from, Point16 to, bool auto)
        {
            Rectangle fromRect = new Rectangle(from.X * 16 - 16, from.Y * 16 - 48, 48, 48);
            Rectangle toRect = new Rectangle(to.X * 16 - 16, to.Y * 16 - 48, 48, 48);
            Vector2 delta = new Vector2(toRect.X - fromRect.X, toRect.Y - fromRect.Y);

            if (!Wiring.blockPlayerTeleportationForOneIteration)
            {
                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].teleporting && fromRect.Intersects(Main.player[i].getRect()))
                    {
                        if (!CanAuto(i) && auto)
                        {
                            continue;
                        }

                        Vector2 newPos = Main.player[i].position + delta;
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

            if (!auto)
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].teleporting && Main.npc[i].lifeMax > 5 && !Main.npc[i].boss && !Main.npc[i].noTileCollide)
                    {
                        int type = Main.npc[i].type;
                        if (!NPCID.Sets.TeleportationImmune[type] && fromRect.Intersects(Main.npc[i].getRect()))
                        {
                            Main.npc[i].Teleport(Main.npc[i].position + delta, 0, 0);
                        }
                    }
                }
            }
        }

        private bool CanAuto(int player)
        {
            TPTrackerWorld tracker = mod.GetModWorld<TPTrackerWorld>();

            if (tracker.autoPrev[player] != ID)
            {
                tracker.autoPrev[player] = GetByPos(Next).ID;
                tracker.autoCooldown[player] = TPTrackerWorld.cooldown;
                return true;
            }
            return false;
        }

        public override void Update()
        {
            if(autoTrigger && isLocked)
            {
                Teleport(Position, Next, true);
            }
        }

        private void SendRequest(TPUnchained.ModMessageID request)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)request);
            packet.Write(ID);
            packet.Send();
        }

        public void SendData()
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)TPUnchained.ModMessageID.ShareTeleporterData);
            packet.Write(ID);
            NetSend(packet, false);
            packet.Send();
        }

        public int GetAddress()
        {
            int add = Main.tile[Position.X - 1, Position.Y].frameY / 18;
            add |= (Main.tile[Position.X, Position.Y].frameY / 18) << 4;
            add |= (Main.tile[Position.X + 1, Position.Y].frameY / 18) << 8;
            return add;
        }

        public static TEWirelessTeleporter GetByPos(Point16 pos)
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

            CheckTriggerState();

            if (isLocked)
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

            TPTrackerWorld tracker = mod.GetModWorld<TPTrackerWorld>();

            if (!tracker.teleporters.Contains(this))
            {
                if (isLocked)
                    tracker.teleporters.Add(this);
            }
            else
            {
                if (!isLocked)
                    tracker.teleporters.Remove(this);
            }
        }
    }
}