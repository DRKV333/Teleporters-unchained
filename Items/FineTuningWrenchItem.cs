using Terraria;
using Terraria.ModLoader;
using TPUnchained.Tiles;

namespace TPUnchained.Items
{
    internal class FineTuningWrenchItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fine-tuning wrench");
            Tooltip.SetDefault("Left click to lock teleporter\nRight click to change order");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 30;
            item.value = 5000; //TBD
            item.maxStack = 1;
            item.useTurn = true;
            item.autoReuse = false;
            item.useAnimation = 15;
            item.useTime = 45;
            item.useStyle = 1;
            item.consumable = false;
            item.mech = true;
        }

        private bool TryGetTarget(out TEWirelessTeleporter TE)
        {
            WirelessTeleporterTile tile = mod.GetTile<WirelessTeleporterTile>();

            return tile.TryGetTE(Player.tileTargetX, Player.tileTargetY, out TE);
        }

        public override bool UseItem(Player player)
        {
            TEWirelessTeleporter target;
            if (TryGetTarget(out target))
            {
                if (target.isLocked)
                    target.Disconnect();
                else
                    target.Connect();
            }
            return true;
        }

        public override void RightClick(Player player)
        {
            TEWirelessTeleporter target;
            if (TryGetTarget(out target))
            {
                if (target.isLocked)
                    target.PushDown();
            }
        }
    }
}