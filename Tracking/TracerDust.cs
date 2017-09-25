using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TPUnchained.Tracking
{
    internal class TracerDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 8, 8);
            dust.customData = (int)10;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            int framesLeft = (int)dust.customData;
            dust.customData = framesLeft - 1;

            if (framesLeft < 0)
            {
                dust.active = false;
            }
            return false;
        }
    }
}