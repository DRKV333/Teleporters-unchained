using Terraria.ModLoader;
using TPUnchained.Tiles;

namespace TPUnchained.Items
{
    public class WirelessTeleporterItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wireless teleporter");
            Tooltip.SetDefault("TBD");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 14;
            item.value = 5000; //TBD
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.mech = true;
            item.createTile = mod.TileType<WirelessTeleporterTile>();
            item.placeStyle = 0;
        }
    }
}