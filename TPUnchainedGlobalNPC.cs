using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TPUnchained.Items;

namespace TPUnchained
{
    class TPUnchainedGlobalNPC : GlobalNPC 
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if(type == NPCID.Steampunker && NPC.downedPlantBoss)
            {
                shop.item[nextSlot++].SetDefaults(mod.ItemType<FineTuningWrenchItem>());
            }
        }
    }
}
