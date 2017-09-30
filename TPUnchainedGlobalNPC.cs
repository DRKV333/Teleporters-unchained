using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TPUnchained.Items;

namespace TPUnchained
{
    internal class TPUnchainedGlobalNPC : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Steampunker && NPC.downedPlantBoss)
            {
                shop.item[nextSlot++].SetDefaults(mod.ItemType<FineTuningWrenchItem>());
            }
        }
    }
}