using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TPUnchained.Tiles;

namespace TPUnchained.Items
{
    public class TeleporterAutotriggerItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Teleporter auto-trigger");
            Tooltip.SetDefault("Place on the side of a Wireless teleporter\nAutomatically teleports players\nDisable with wire");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 30;
            item.value = Item.sellPrice(0, 2, 0, 0);
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

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.LogicSensor_Above, 1);
            recipe.AddIngredient(ItemID.Wire, 5);
            recipe.AddIngredient(ItemID.Ectoplasm, 3);

            recipe.SetResult(item.type, 1);

            recipe.AddTile(TileID.MythrilAnvil);

            recipe.AddRecipe();
        }
    }
}