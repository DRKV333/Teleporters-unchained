using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TPUnchained.Tiles;

namespace TPUnchained.Items
{
    class TeleporterAutotriggerItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Teleporter auto trigger");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 30;
            item.value = Item.buyPrice(0, 10, 0, 0);
            item.rare = 9;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.mech = true;
            item.createTile = mod.TileType<TeleporterAutotriggerTile>();
            item.placeStyle = 0;
        }
    }
}
