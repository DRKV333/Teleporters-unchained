using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TPUnchained.Items;

namespace TPUnchained.Tiles
{
    public class WirelessTeleporterTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEWirelessTeleporter>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.addTile(Type);

            ModTranslation label = CreateMapEntryName();
            label.AddTranslation(GameCulture.English, "Wireless teleporter");
            AddMapEntry(new Color(46, 184, 214), label);
        }

        public bool TryGetTE(int i, int j, out TEWirelessTeleporter te)
        {
            te = null;
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.active() && tile.type != Type)
                return false;

            int originX = (i - tile.frameX % 54 / 18) + 1;
            int TEId = mod.GetTileEntity<TEWirelessTeleporter>().Find(originX, j);

            if (TEId == -1)
                return false;

            te = (TEWirelessTeleporter)TileEntity.ByID[TEId];

            return true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
                SetSlotItem(i, j, 0);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int originX = (i - frameX % 54 / 18) + 1;
            Item.NewItem(originX * 16, j * 16, 16, 16, mod.ItemType<WirelessTeleporterItem>());
            mod.GetTileEntity<TEWirelessTeleporter>().Kill(originX, j);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 20)
            {
                frameCounter = 0;
                frame++;
                if (frame > 3)
                {
                    frame = 0;
                }
                Main.tileLighted[Type] = frame > 1;
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            TEWirelessTeleporter TE;
            if (!TryGetTE(i, j, out TE))
                return;

            if (TE.isLocked)
            {
                frameYOffset = 0;
                frameXOffset = Main.tileFrame[type] * 54;
            }
            else
            {
                frameYOffset = 0;
                frameXOffset = 162;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            TEWirelessTeleporter TE;
            if (!TryGetTE(i, j, out TE))
                return;

            if (TE.isLocked && Main.tileLighted[Type])
            {
                r = 0; g = 0.6f; b = 0.6f;
            }
            else
            {
                r = 0; g = 0; b = 0;
            }
        }

        public override void RightClick(int i, int j)
        {
            TEWirelessTeleporter TE;
            if (!TryGetTE(i, j, out TE) || TE.isLocked)
                return;

            Tile tile = Main.tile[i, j];

            if (SetSlotItem(i, j, Main.LocalPlayer.HeldItem.type))
            {
                Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].stack--;
                if (Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].stack <= 0)
                {
                    Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].SetDefaults(0, false);
                    Main.mouseItem.SetDefaults(0, false);
                }
                if (Main.LocalPlayer.selectedItem == 58)
                {
                    Main.mouseItem = Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].Clone();
                }
            }
        }

        public override void HitWire(int i, int j)
        {
            TEWirelessTeleporter TE;
            if (!TryGetTE(i, j, out TE))
                return;

            Wiring.SkipWire(TE.Position.X - 1, TE.Position.Y);
            Wiring.SkipWire(TE.Position.X, TE.Position.Y);
            Wiring.SkipWire(TE.Position.X + 1, TE.Position.Y);

            if (TE.isLocked)
                TE.Teleport();
        }

        public override void MouseOver(int i, int j)
        {
            Main.LocalPlayer.noThrow = 2;
        }

        public int GetSlotItem(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile != null && tile.active() && tile.type == mod.TileType<WirelessTeleporterTile>())
            {
                switch (tile.frameY)
                {
                    case 0: return 0;
                    case 18: return ItemID.Amethyst;
                    case 36: return ItemID.Topaz;
                    case 54: return ItemID.Sapphire;
                    case 72: return ItemID.Emerald;
                    case 90: return ItemID.Ruby;
                    case 108: return ItemID.Diamond;
                    case 126: return ItemID.Amber;
                }
            }
            return 0;
        }

        public bool SetSlotItem(int i, int j, int type)
        {
            bool success = false;

            int prevType = GetSlotItem(i, j);
            if (prevType != 0)
            {
                Item.NewItem(i * 16, j * 16, 18, 18, prevType);
            }

            Tile tile = Main.tile[i, j];
            if (tile != null && tile.active() && tile.type == mod.TileType<WirelessTeleporterTile>())
            {
                switch (type)
                {
                    case ItemID.Amethyst: tile.frameY = 18; success = true; break;
                    case ItemID.Topaz: tile.frameY = 36; success = true; break;
                    case ItemID.Sapphire: tile.frameY = 54; success = true; break;
                    case ItemID.Emerald: tile.frameY = 72; success = true; break;
                    case ItemID.Ruby: tile.frameY = 90; success = true; break;
                    case ItemID.Diamond: tile.frameY = 108; success = true; break;
                    case ItemID.Amber: tile.frameY = 126; success = true; break;
                    default: tile.frameY = 0; break;
                }
            }

            if (Main.netMode == 1 && success)
            {
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
            }

            return success;
        }
    }
}