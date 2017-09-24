using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TPUnchained.Tiles;

namespace TPUnchained.Items
{
    public class WirelessTeleporterItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wireless teleporter");
            Tooltip.SetDefault("Right click on slots with gems to set address\nUse Fine-tuning wrench to lock");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 14;
            item.value = Item.buyPrice(0, 10, 0, 0);
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

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.Teleporter, 1);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.Ectoplasm, 8);

            recipe.SetResult(item.type, 1);

            recipe.AddTile(TileID.MythrilAnvil);

            recipe.AddRecipe();
        }
    }
}