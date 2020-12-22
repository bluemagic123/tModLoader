﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraria.ModLoader.Default.Developer.Jofairden
{
	[AutoloadEquip(EquipType.Head)]
	internal class PowerRanger_Head : AndromedonItem
	{
		public override void SetDefaults() {
			base.SetDefaults();
			item.Size = new Vector2(18, 20);
		}

		public override bool IsVanitySet(int head, int body, int legs) {
			return head == Mod.GetEquipSlot(nameof(PowerRanger_Head), EquipType.Head)
				   && body == Mod.GetEquipSlot(nameof(PowerRanger_Body), EquipType.Body)
				   && legs == Mod.GetEquipSlot(nameof(PowerRanger_Legs), EquipType.Legs);
		}

		public override void UpdateVanitySet(Player player) {
			DeveloperPlayer.GetPlayer(player).AndromedonEffect.HasSetBonus = true;
		}

		private static Texture2D _glowTexture;
		private static Texture2D _shaderTexture;

		public static PlayerLayer GlowLayer = CreateGlowLayer("AndromedonHeadGlow", PlayerLayer.Head, drawInfo => {
			_glowTexture ??= ModLoaderMod.ReadTexture($"Developer.PowerRanger_Head_Head_Glow");
			return GetHeadDrawDataInfo(drawInfo, _glowTexture);
		});

		public static PlayerLayer ShaderLayer = CreateShaderLayer("AndromedonHeadShader", PlayerLayer.Body, drawInfo => {
			_shaderTexture ??= ModLoaderMod.ReadTexture($"Developer.PowerRanger_Head_Head_Shader");
			return GetHeadDrawDataInfo(drawInfo, _shaderTexture);
		});
	}
}
