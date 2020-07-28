using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Content.Items.Weapons
{
	public class ExampleMagicWeapon : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("This is an example magic weapon");
		}
		public override void SetDefaults()
		{
			item.damage = 25;
			item.magic = true; // makes the damage register as magic
      // important note: if your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). be sure to have a damage type
			item.width = 34;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.HoldingOut; // the player holds out the item
			item.noMelee = true; // makes the item not do damage with it's melee hitbox
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 4;
			item.UseSound = SoundID.Item71;
			item.autoReuse = true;
			item.shoot = ProjectileID.BlackBolt; //shoot a black bolt, also known as the projectile shot from the onyx blaster
			item.shootSpeed = 7; // how fast the item shoots the projectile
			item.crit = 32; // % chance at a crit, plus the default amount
			item.mana = 11; //this is how much mana the item uses
		}
		public override void AddRecipes()
		{
        CreateRecipe(51)
            .AddIngredient(ItemID.FallenStar, 5)
            .AddIngredient<ExampleItem>()
            .AddTile<ExampleWorkbench>()
            .Register();
            
		}
	}
}
