using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.NPCs
{
	// This NPC inherits from the HoverNPC abstract class included in ExampleMod, which is a customizable AI similar to that of Vanilla's Hover AI.
	// The CustomBehavior and ShouldMove virtual methods are overridden here, as well as the acceleration and accelerationY field being set in the class constructor.
	public class FallenSoul : HoverNPC
	{
		public FallenSoul() {
			accelerationX = 0.06f;
			accelerationY = 0.025f;
		}

		public override void SetStaticDefaults() => Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.Wraith];

		public override void SetDefaults() {
			npc.CloneDefaults(NPCID.Wraith); // Clone the default stats of a Wraith.
			// Here we adjust the stats to our liking.
			npc.width = 28;
			npc.height = 36;
			npc.aiStyle = -1;
			npc.damage = 0;
			npc.friendly = true;
			npc.dontTakeDamageFromHostiles = true;
			animationType = NPCID.Wraith;
		}

		// Allows hitting the NPC with items, even if it's friendly.
		public override bool? CanBeHitByItem(Player player, Item item) => true;

		// Allows hitting the NPC with friendly projectiles.
		public override bool? CanBeHitByProjectile(Projectile projectile) => projectile.friendly && projectile.owner < 255;

		// Allows us to do something when the NPC is hit. Here, we spawn dust.
		public override void HitEffect(int hitDirection, double damage) {
			int loopAmount = 50;
			int speedX = hitDirection * 2;
			float speedY = -2f;

			// If the NPC is alive when it gets hit, we change the variables.
			if (npc.life > 0) {
				loopAmount = (int)(damage / npc.lifeMax * 100);
				speedX = hitDirection;
				speedY = -1f;
			}

			// Loop as many times as loopAmount is set ot.
			for (int i = 0; i < loopAmount; i++) {
				// Here we spawn dust that uses the variables defined earlier in this method. This will spawn when the NPC is hit.
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 192, speedX, speedY, 100, new Color(100, 100, 100, 100), 1f);

				// Makes it so the dust we spawned is not affected by gravity.
				dust.noGravity = true;
			}
		}

		// Allows the NPC to talk with the player, even if it isn't a town NPC.
		public override bool? CanChat() => true;

		// Allows us to choose what the NPC says when talked to.
		public override string GetChat() {
			// npc.SpawnedFromStatue value is kept when the NPC is transformed.
			switch (Main.rand.Next(npc.SpawnedFromStatue ? 5 : 3)) {
				case 0:
					return "Thank you, now I don't have to haunt random people anymore, only you.";
				case 1:
					return "Keep breaking those evil altars, me and many others were cursed to haunt anyone who did so.";
				case 2:
					return "Can you help me get into heaven?";
				default:
					return "Please stop messing with that haunted statue. Don't you know what \"RIP\" means?";
			}
		}

		// Allows us to set when our chat buttons will say.
		public override void SetChatButtons(ref string button, ref string button2) => button = "Send to heaven";

		// Allows us to do stuff depending on which button was pressed.
		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			if (firstButton) {
				// Hit the NPC for about 500 damage if the first button is clicked.
				Main.LocalPlayer.ApplyDamageToNPC(npc, Main.DamageVar(500), 5f, Main.LocalPlayer.direction, true);
			}
		}

		// Only draw the NPC's health bar when the player is at a certain distance or closer.
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
			float distance = npc.Distance(Main.player[npc.target].Center);

			if (distance <= 200) {
				if (distance > 100) {
					// Make the health bar become smaller the farther away the NPC is.
					scale *= (100 - (distance - 100)) / 100;
				}

				return null;
			}

			return false;
		}

		// Make the NPC invisible when far away from the player.
		// CustomBehavior is a custom hook in HoverNPC.cs.
		public override void CustomBehavior(ref float ai) {
			float distance = npc.Distance(Main.player[npc.target].Center);

			if (distance <= 250) {
				npc.alpha = 100;

				if (distance > 100) {
					// Make the NPC fade out the farther away the NPC is.
					npc.alpha += (int)(155 * ((distance - 100) / 150));
				}

				return;
			}

			npc.alpha = 255;
		}

		// Make the NPC stop moving if it is close to the player.
		// ShouldMove is a custom hook in HoverNPC.cs.
		public override bool ShouldMove(float ai) {
			npc.ai[2] = 0; // Prevents the NPC from stopping when following its target.

			if (npc.Distance(Main.player[npc.target].Center) < 150f) {
				npc.velocity *= 0.95f;

				if (Math.Abs(npc.velocity.X) < 0.1f) {
					npc.spriteDirection = Main.player[npc.target].Center.X > npc.Center.X ? 1 : -1;
					npc.velocity.X = 0;
				}

				return false;
			}

			return true;
		}
	}

	// This is an example of a GlobalProjectile class. GlobalProjectile hooks are called on all projectiles in the game and are suitable for sweeping
	// changes like adding additional data to all projectiles in the game. Here we simply override PostAI in order to make Purification Powder loop through the
	// NPC array to affect Wraiths specifically, transforming them into Fallen Souls, as it is simple to understand.
	public class WraithPurificationGlobalProjectile : GlobalProjectile
	{
		// Make purification powder transform wraiths into purified ghosts.
		public override void PostAI(Projectile projectile) {
			if (projectile.type == ProjectileID.PurificationPowder && Main.netMode != NetmodeID.MultiplayerClient) {
				for (int i = 0; i < Main.maxNPCs; i++) {
					if (Main.npc[i].active && Main.npc[i].type == NPCID.Wraith && projectile.Hitbox.Intersects(Main.npc[i].Hitbox)) {
						Main.npc[i].Transform(ModContent.NPCType<FallenSoul>());
					}
				}
			}
		}
	}
}
