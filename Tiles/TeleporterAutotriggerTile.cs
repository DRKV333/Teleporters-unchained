﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TPUnchained.Items;

namespace TPUnchained.Tiles
{
    class TeleporterAutotriggerTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            dustType = 1;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newTile.AnchorValidTiles = new int[] { mod.TileType<WirelessTeleporterTile>() };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile, TileObjectData.newAlternate.Height, 0);
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            drop = mod.ItemType<TeleporterAutotriggerItem>();

            AddMapEntry(new Color(255, 255, 255));
        }
    }
}