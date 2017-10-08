using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TPUnchained.Tiles;

namespace TPUnchained
{
    internal class ChestPlacementFix : GlobalTile
    {
        public override bool CanPlace(int i, int j, int type)
        {
            if (TileID.Sets.BasicChest[type] || TileID.Sets.BasicChestFake[type] || TileLoader.IsDresser(type))
            {
                Tile bottom = Main.tile[i, j + 1];
                if (bottom != null && bottom.active() && bottom.type == mod.TileType<WirelessTeleporterTile>())
                {
                    return false;
                }
            }
            return true;
        }
    }
}