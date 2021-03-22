using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ExampleMod.Projectiles;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Items.Weapons
{
	public class ExampleFlamethrower : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.Flamethrower;

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Shoots a jet of cursed flames\nHas a 66% chance to not consume gel\nThis is a modded flamethrower.");
		}

		public override void SetDefaults()
		{
			item.damage = 69; //The item's damage.
			item.ranged = true;
			item.width = 54;
			item.height = 16;
			item.useTime = 4;
			item.useAnimation = 20; //Shoots out 5 jets of fire in one shot
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true; //So the item's animation doesn't do damage
			item.knockBack = 2;
			item.value = 10000;
			item.color = new Color(61, 252, 3); //Makes the item color green.
			item.rare = ItemRarityID.Green; //Sets the item's rarity.
			item.UseSound = SoundID.Item34;
			item.autoReuse = true;
            item.shoot = ProjectileType<FlamethrowerProj>();
			item.shootSpeed = 21f; //How fast the flames will travel
			item.useAmmo = AmmoID.Gel; //Makes the weapon use up Gel as ammo
		}
		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .66f;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Flamethrower);
			recipe.AddIngredient(ItemType<ExampleItem>(), 3);
			recipe.AddIngredient(ItemID.CursedFlame, 15);
			recipe.AddTile(TileType<Tiles.ExampleWorkbench>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 54f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			//This is to prevent shooting through blocks and to make the fire shoot from the muzzle.
			return true;
        }
        public override Vector2? HoldoutOffset()
		{
			return new Vector2(0, 0); //If your own flamethrower is being held wrong, edit these values. You can test out holdout offsets using Modder's Toolkit.
		}
	}
}