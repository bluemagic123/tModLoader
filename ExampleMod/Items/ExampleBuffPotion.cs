using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items
{
	public class ExampleBuffPotion : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Gives a light defense buff.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 30;
            item.consumable = true;
            item.rare = 3;
            item.buffType = mod.BuffType("ExampleDefenseBuff"); //Specify an existing buff to be applied when used.
            item.buffTime = 5400; //The amount of time the buff declared in item.buffType will last in ticks.
            item.value = Item.buyPrice(gold: 1);
        }
    }
}
